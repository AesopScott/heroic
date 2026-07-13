#!/usr/bin/env node

import fs from "node:fs/promises";
import path from "node:path";
import process from "node:process";

const DEFAULT_MANIFEST = "UpgradeSkills.md";
const DEFAULT_OUTPUT_DIR = "Assets/Resources/SkillIcons/Generated";
const DEFAULT_MASTER_DIR = process.env.HEROIC_ICON_MASTER_DIR || "Assets/_Heroic/Art/IconMasters";
const DEFAULT_LOG = "Assets/_Heroic/Art/icon-generation-log.json";
const DEFAULT_MODEL = process.env.HEROIC_ICON_MODEL || "gpt-image-1.5";
const DEFAULT_SIZE = process.env.HEROIC_ICON_SIZE || "1024x1024";
const DEFAULT_QUALITY = process.env.HEROIC_ICON_QUALITY || "medium";
const DEFAULT_CONCURRENCY = Number.parseInt(process.env.HEROIC_ICON_CONCURRENCY || "2", 10);
const DEFAULT_CHROMA_KEY = process.env.HEROIC_ICON_CHROMA_KEY || "#FE00FF";
const DEFAULT_RUNTIME_SIZE = Number.parseInt(process.env.HEROIC_ICON_RUNTIME_SIZE || "256", 10);
const DEFAULT_REQUEST_SPACING_MS = Number.parseInt(process.env.HEROIC_ICON_REQUEST_SPACING_MS || "12500", 10);

const HEROIC_STYLE_PROMPT = [
  "Create a single polished square game icon for Heroic representing: {description}.",
  "Visual language: dark heroic fantasy spellcraft, crisp readable silhouette, premium Unity action-RPG HUD asset, painterly but clean, luminous magical materials, controlled rim light, subtle bevel depth, high contrast, centered subject, consistent three-quarter icon perspective, consistent line weight, generous padding.",
  "Use the requested chroma-key color as one flat solid background only: {chromaKey}. The subject must not use that chroma-key color or any near match.",
  "No words, no letters, no numbers, no watermark, no UI frame, no surrounding scene, no mockup, no unrelated objects."
].join(" ");

function parseArgs(argv) {
  const args = {
    manifest: DEFAULT_MANIFEST,
    outputDir: DEFAULT_OUTPUT_DIR,
    masterDir: DEFAULT_MASTER_DIR,
    log: DEFAULT_LOG,
    model: DEFAULT_MODEL,
    size: DEFAULT_SIZE,
    quality: DEFAULT_QUALITY,
    concurrency: Number.isFinite(DEFAULT_CONCURRENCY) && DEFAULT_CONCURRENCY > 0 ? DEFAULT_CONCURRENCY : 2,
    requestSpacingMs: Number.isFinite(DEFAULT_REQUEST_SPACING_MS) && DEFAULT_REQUEST_SPACING_MS >= 0 ? DEFAULT_REQUEST_SPACING_MS : 12500,
    chromaKey: DEFAULT_CHROMA_KEY,
    runtimeSize: Number.isFinite(DEFAULT_RUNTIME_SIZE) && DEFAULT_RUNTIME_SIZE > 0 ? DEFAULT_RUNTIME_SIZE : 256,
    force: false,
    dryRun: false,
    validate: false,
    failedOnly: false,
    updateManifestImages: false,
    id: "",
    category: "",
    retries: 3,
    tolerance: 58,
    edgeSoftness: 72
  };

  for (let i = 0; i < argv.length; i++) {
    const arg = argv[i];
    const next = () => argv[++i];
    switch (arg) {
      case "--manifest":
        args.manifest = next();
        break;
      case "--output-dir":
        args.outputDir = next();
        break;
      case "--master-dir":
        args.masterDir = next();
        break;
      case "--log":
        args.log = next();
        break;
      case "--model":
        args.model = next();
        break;
      case "--size":
        args.size = next();
        break;
      case "--quality":
        args.quality = next();
        break;
      case "--concurrency":
        args.concurrency = Math.max(1, Number.parseInt(next(), 10));
        break;
      case "--request-spacing-ms":
        args.requestSpacingMs = Math.max(0, Number.parseInt(next(), 10));
        break;
      case "--chroma-key":
        args.chromaKey = next();
        break;
      case "--runtime-size":
        args.runtimeSize = Math.max(16, Number.parseInt(next(), 10));
        break;
      case "--id":
        args.id = next();
        break;
      case "--category":
        args.category = normalizeCategory(next());
        break;
      case "--retries":
        args.retries = Math.max(0, Number.parseInt(next(), 10));
        break;
      case "--tolerance":
        args.tolerance = Math.max(0, Number.parseInt(next(), 10));
        break;
      case "--edge-softness":
        args.edgeSoftness = Math.max(1, Number.parseInt(next(), 10));
        break;
      case "--force":
        args.force = true;
        break;
      case "--dry-run":
        args.dryRun = true;
        break;
      case "--validate":
        args.validate = true;
        break;
      case "--failed-only":
        args.failedOnly = true;
        break;
      case "--update-manifest-images":
        args.updateManifestImages = true;
        break;
      case "--help":
      case "-h":
        printHelp();
        process.exit(0);
        break;
      default:
        throw new Error(`Unknown argument: ${arg}`);
    }
  }

  return args;
}

function printHelp() {
  console.log(`Heroic icon generation pipeline

Usage:
  node scripts/generate-icons.mjs [options]

Options:
  --validate                  Validate the manifest and exit.
  --dry-run                   Show planned work without API calls.
  --force                     Regenerate icons even when output exists.
  --failed-only               Regenerate only icons with failed log entries.
  --id <icon-id>              Generate one icon by ID.
  --category <category>       Generate one category.
  --manifest <path>           Manifest path. Default: ${DEFAULT_MANIFEST}
  --output-dir <path>         Output root. Default: ${DEFAULT_OUTPUT_DIR}
  --master-dir <path>         1024 master output root. Default: ${DEFAULT_MASTER_DIR}
  --log <path>                JSON log path. Default: ${DEFAULT_LOG}
  --model <model>             Image model. Default: ${DEFAULT_MODEL}
  --quality <quality>         Image quality. Default: ${DEFAULT_QUALITY}
  --size <size>               Image size. Default: ${DEFAULT_SIZE}
  --concurrency <n>           Parallel requests. Default: ${DEFAULT_CONCURRENCY}
  --request-spacing-ms <ms>   Minimum delay between API request starts. Default: ${DEFAULT_REQUEST_SPACING_MS}
  --chroma-key <hex>          Flat background color. Default: ${DEFAULT_CHROMA_KEY}
  --runtime-size <px>         Runtime PNG size. Default: ${DEFAULT_RUNTIME_SIZE}
  --update-manifest-images    Add/update Markdown image previews in the manifest.
  --retries <n>               Retry count. Default: 3
`);
}

async function main() {
  const args = parseArgs(process.argv.slice(2));
  const manifestPath = path.resolve(args.manifest);
  const icons = await readIconManifest(manifestPath);
  validateIcons(icons, manifestPath);

  if (args.updateManifestImages) {
    await updateMarkdownManifestImages(manifestPath, icons, args);
    console.log(`Updated manifest image previews: ${args.manifest}`);
    return;
  }

  if (args.validate) {
    console.log(`Manifest valid: ${icons.length} icon definitions found in ${args.manifest}`);
    printCategorySummary(icons);
    return;
  }

  const previousLog = await readGenerationLog(args.log);
  const selected = await selectIcons(icons, args, previousLog);
  printPlan(selected, args);

  if (args.dryRun) {
    return;
  }

  if (!process.env.OPENAI_API_KEY) {
    throw new Error("OPENAI_API_KEY is not set. Export it in the environment before generating icons.");
  }

  const OpenAI = (await import("openai")).default;
  const sharp = (await import("sharp")).default;
  const client = new OpenAI({ apiKey: process.env.OPENAI_API_KEY });
  const results = [];

  await runWithConcurrency(selected, args.concurrency, async (icon) => {
    const result = await generateOneIcon({ icon, args, client, sharp });
    results.push(result);
    await appendGenerationLog(args.log, result);
    const label = result.status === "success" ? "generated" : "failed";
    console.log(`${label}: ${icon.id} -> ${result.filename}`);
  });

  const failures = results.filter((result) => result.status !== "success").length;
  if (failures > 0) {
    process.exitCode = 1;
  }
}

async function readIconManifest(manifestPath) {
  const text = await fs.readFile(manifestPath, "utf8");
  if (manifestPath.toLowerCase().endsWith(".json")) {
    return parseJsonManifest(JSON.parse(text));
  }

  return parseMarkdownManifest(text);
}

function parseMarkdownManifest(text) {
  const icons = [];
  let section = "";
  let group = "";
  let subgroup = "";

  for (const rawLine of text.split(/\r?\n/)) {
    const line = rawLine.trim();
    if (line.startsWith("## ")) {
      section = line.slice(3).trim();
      group = "";
      subgroup = "";
      continue;
    }

    if (line.startsWith("### ")) {
      group = line.slice(4).trim();
      subgroup = "";
      continue;
    }

    if (line.startsWith("#### ")) {
      subgroup = line.slice(5).trim();
      continue;
    }

    const match = line.match(/^- `([^`]+)`\s+-\s+(.+?)(?:\s+!\[[^\]]*\]\([^)]+\))?$/);
    if (!match) {
      continue;
    }

    const id = match[1].trim();
    const label = match[2].trim();
    const category = categoryFromMarkdownSection(section);
    const subject = [group, subgroup].filter(Boolean).join(" / ");
    const description = subject ? `${label} (${subject})` : label;
    icons.push({
      id,
      label,
      description,
      category,
      source: "UpgradeSkills.md"
    });
  }

  return icons;
}

function parseJsonManifest(json) {
  const icons = [];
  for (const [category, value] of Object.entries(json)) {
    if (!Array.isArray(value)) {
      continue;
    }

    for (const entry of value) {
      if (!entry || typeof entry.key !== "string") {
        continue;
      }

      const label = labelFromKey(entry.key);
      const colorText = entry.color ? ` Heroic palette color ${entry.color}.` : "";
      icons.push({
        id: entry.key,
        label,
        description: `${label}.${colorText}`.trim(),
        category: normalizeCategory(category),
        source: "json-manifest",
        color: entry.color || ""
      });
    }
  }

  return icons;
}

function categoryFromMarkdownSection(section) {
  const normalized = normalizeCategory(section.replace(/ Upgrades$/i, ""));
  if (normalized === "ability") return "ability";
  if (normalized === "movement") return "movement";
  if (normalized === "system-synergy") return "system-synergy";
  if (normalized === "system") return "system";
  return normalized || "uncategorized";
}

function normalizeCategory(value) {
  return String(value || "")
    .trim()
    .toLowerCase()
    .replace(/&/g, "and")
    .replace(/[^a-z0-9]+/g, "-")
    .replace(/^-+|-+$/g, "");
}

function safeFilename(id) {
  return String(id)
    .trim()
    .toLowerCase()
    .replace(/[^a-z0-9._-]+/g, "_")
    .replace(/^_+|_+$/g, "") + ".png";
}

function labelFromKey(key) {
  return key
    .split(/[._-]+/g)
    .filter(Boolean)
    .map((part) => part.charAt(0).toUpperCase() + part.slice(1))
    .join(" ");
}

function validateIcons(icons, manifestPath) {
  if (icons.length === 0) {
    throw new Error(`No icon definitions found in ${manifestPath}`);
  }

  const seen = new Set();
  const duplicates = [];
  for (const icon of icons) {
    if (!icon.id || !icon.description || !icon.category) {
      throw new Error(`Invalid icon definition: ${JSON.stringify(icon)}`);
    }

    if (seen.has(icon.id)) {
      duplicates.push(icon.id);
    }

    seen.add(icon.id);
  }

  if (duplicates.length > 0) {
    throw new Error(`Duplicate icon IDs found: ${duplicates.join(", ")}`);
  }
}

async function selectIcons(icons, args, previousLog) {
  let selected = icons;
  if (args.id) {
    selected = selected.filter((icon) => icon.id === args.id);
    if (selected.length === 0) {
      throw new Error(`No icon found with ID: ${args.id}`);
    }
  }

  if (args.category) {
    selected = selected.filter((icon) => icon.category === args.category);
    if (selected.length === 0) {
      throw new Error(`No icons found in category: ${args.category}`);
    }
  }

  if (args.failedOnly) {
    const failedIds = new Set(previousLog.filter((entry) => entry.status === "failed").map((entry) => entry.iconId));
    selected = selected.filter((icon) => failedIds.has(icon.id));
  }

  const withPaths = selected.map((icon) => {
    const filename = path.join(args.outputDir, icon.category, safeFilename(icon.id));
    const masterFilename = path.join(args.masterDir, icon.category, safeFilename(icon.id));
    const prompt = buildPrompt(icon, args.chromaKey);
    return { ...icon, filename, masterFilename, prompt };
  });

  if (args.force || args.failedOnly) {
    return withPaths;
  }

  const missing = [];
  for (const icon of withPaths) {
    if (!(await exists(icon.filename))) {
      missing.push(icon);
    }
  }

  return missing;
}

function buildPrompt(icon, chromaKey) {
  return HEROIC_STYLE_PROMPT
    .replace("{description}", icon.description)
    .replaceAll("{chromaKey}", normalizeHexColor(chromaKey));
}

async function generateOneIcon({ icon, args, client, sharp }) {
  const timestamp = new Date().toISOString();
  let attempt = 0;
  let lastError = null;

  while (attempt <= args.retries) {
    attempt++;
    try {
      await waitForRequestSlot(args);
      const response = await client.images.generate({
        model: args.model,
        prompt: icon.prompt,
        size: args.size,
        quality: args.quality,
        n: 1
      });

      const b64 = extractBase64Image(response);
      const generatedBuffer = Buffer.from(b64, "base64");
      const masterBuffer = await removeChromaKey(generatedBuffer, {
        sharp,
        chromaKey: args.chromaKey,
        tolerance: args.tolerance,
        edgeSoftness: args.edgeSoftness
      });
      const runtimeBuffer = await sharp(masterBuffer)
        .resize(args.runtimeSize, args.runtimeSize, {
          fit: "contain",
          kernel: "lanczos3",
          background: { r: 0, g: 0, b: 0, alpha: 0 }
        })
        .png({ compressionLevel: 9 })
        .toBuffer();

      await fs.mkdir(path.dirname(icon.masterFilename), { recursive: true });
      await fs.mkdir(path.dirname(icon.filename), { recursive: true });
      await fs.writeFile(icon.masterFilename, masterBuffer);
      await fs.writeFile(icon.filename, runtimeBuffer);
      return {
        iconId: icon.id,
        filename: path.normalize(icon.filename),
        masterFilename: path.normalize(icon.masterFilename),
        prompt: icon.prompt,
        model: args.model,
        status: "success",
        attemptCount: attempt,
        generationTimestamp: timestamp,
        runtimeSize: args.runtimeSize
      };
    } catch (error) {
      lastError = serializeError(error);
      if (attempt > args.retries || !isTemporaryFailure(error)) {
        break;
      }

      await sleep(backoffMs(attempt));
    }
  }

  return {
    iconId: icon.id,
    filename: path.normalize(icon.filename),
    masterFilename: path.normalize(icon.masterFilename),
    prompt: icon.prompt,
    model: args.model,
    status: "failed",
    attemptCount: attempt,
    generationTimestamp: timestamp,
    error: lastError
  };
}

function extractBase64Image(response) {
  const first = response?.data?.[0];
  if (first?.b64_json) {
    return first.b64_json;
  }

  const outputImage = response?.output?.find?.((item) => item.type === "image_generation_call");
  if (outputImage?.result) {
    return outputImage.result;
  }

  throw new Error("OpenAI Images response did not include base64 image data.");
}

async function removeChromaKey(inputBuffer, options) {
  const { sharp, chromaKey, tolerance, edgeSoftness } = options;
  const key = hexToRgb(chromaKey);
  const image = sharp(inputBuffer).ensureAlpha();
  const metadata = await image.metadata();
  const { data, info } = await image.raw().toBuffer({ resolveWithObject: true });

  for (let i = 0; i < data.length; i += info.channels) {
    const dr = data[i] - key.r;
    const dg = data[i + 1] - key.g;
    const db = data[i + 2] - key.b;
    const distance = Math.sqrt(dr * dr + dg * dg + db * db);
    const alpha = alphaFromDistance(distance, tolerance, edgeSoftness);
    data[i + 3] = Math.min(data[i + 3], alpha);

    if (alpha < 255) {
      data[i] = Math.round(data[i] * (alpha / 255));
      data[i + 1] = Math.round(data[i + 1] * (alpha / 255));
      data[i + 2] = Math.round(data[i + 2] * (alpha / 255));
    }
  }

  return sharp(data, {
    raw: {
      width: info.width,
      height: info.height,
      channels: info.channels
    }
  })
    .png({ compressionLevel: 9 })
    .withMetadata({
      density: metadata.density
    })
    .toBuffer();
}

function alphaFromDistance(distance, tolerance, edgeSoftness) {
  if (distance <= tolerance) {
    return 0;
  }

  if (distance >= tolerance + edgeSoftness) {
    return 255;
  }

  return Math.round(((distance - tolerance) / edgeSoftness) * 255);
}

async function runWithConcurrency(items, concurrency, worker) {
  let cursor = 0;
  const workers = Array.from({ length: Math.min(concurrency, items.length) }, async () => {
    while (cursor < items.length) {
      const index = cursor++;
      await worker(items[index], index);
    }
  });

  await Promise.all(workers);
}

async function readGenerationLog(logPath) {
  try {
    const text = await fs.readFile(logPath, "utf8");
    const parsed = JSON.parse(text);
    return Array.isArray(parsed) ? parsed : [];
  } catch (error) {
    if (error.code === "ENOENT") {
      return [];
    }

    throw error;
  }
}

async function appendGenerationLog(logPath, entry) {
  const existing = await readGenerationLog(logPath);
  existing.push(entry);
  await fs.mkdir(path.dirname(logPath), { recursive: true });
  await fs.writeFile(logPath, JSON.stringify(existing, null, 2) + "\n", "utf8");
}

function printCategorySummary(icons) {
  const counts = new Map();
  for (const icon of icons) {
    counts.set(icon.category, (counts.get(icon.category) || 0) + 1);
  }

  for (const [category, count] of [...counts.entries()].sort()) {
    console.log(`  ${category}: ${count}`);
  }
}

function printPlan(icons, args) {
  console.log(`Manifest: ${args.manifest}`);
  console.log(`Output: ${args.outputDir}`);
  console.log(`Model: ${args.model}`);
  console.log(`Size: ${args.size}`);
  console.log(`Quality: ${args.quality}`);
  console.log(`Concurrency: ${args.concurrency}`);
  console.log(`Request spacing: ${args.requestSpacingMs}ms`);
  console.log(`Chroma key: ${normalizeHexColor(args.chromaKey)}`);
  console.log(`Master output: ${args.masterDir}`);
  console.log(`Runtime size: ${args.runtimeSize}x${args.runtimeSize}`);
  console.log(`Planned icons: ${icons.length}`);
  printCategorySummary(icons);
  for (const icon of icons.slice(0, 20)) {
    console.log(`  ${icon.id} -> ${icon.filename}`);
  }

  if (icons.length > 20) {
    console.log(`  ... ${icons.length - 20} more`);
  }
}

async function updateMarkdownManifestImages(manifestPath, icons, args) {
  if (!manifestPath.toLowerCase().endsWith(".md")) {
    throw new Error("--update-manifest-images is only supported for Markdown manifests.");
  }

  const byId = new Map();
  for (const icon of icons) {
    const filename = path.join(args.outputDir, icon.category, safeFilename(icon.id)).replaceAll("\\", "/");
    byId.set(icon.id, { ...icon, filename });
  }

  const text = await fs.readFile(manifestPath, "utf8");
  const lines = text.split(/\r?\n/).map((line) => {
    const match = line.match(/^(- `([^`]+)`\s+-\s+.*?)(?:\s+!\[[^\]]*\]\([^)]+\))?$/);
    if (!match) {
      return line;
    }

    const icon = byId.get(match[2]);
    if (!icon) {
      return line;
    }

    const alt = icon.label.replace(/[\[\]()]/g, "");
    return `${match[1]} ![${alt}](${icon.filename})`;
  });

  await fs.writeFile(manifestPath, lines.join("\n"), "utf8");
}

function isTemporaryFailure(error) {
  const status = error?.status || error?.code;
  return status === 408 || status === 409 || status === 429 || (typeof status === "number" && status >= 500);
}

function serializeError(error) {
  return {
    name: error?.name || "Error",
    message: error?.message || String(error),
    status: error?.status || error?.code || null
  };
}

function backoffMs(attempt) {
  const jitter = Math.floor(Math.random() * 250);
  return Math.min(30000, 1000 * 2 ** (attempt - 1)) + jitter;
}

function sleep(ms) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

async function exists(filePath) {
  try {
    await fs.access(filePath);
    return true;
  } catch {
    return false;
  }
}

function normalizeHexColor(value) {
  const color = String(value || "").trim();
  if (/^#[0-9a-fA-F]{6}$/.test(color)) {
    return color.toUpperCase();
  }

  throw new Error(`Invalid chroma-key color: ${value}`);
}

function hexToRgb(value) {
  const color = normalizeHexColor(value).slice(1);
  return {
    r: Number.parseInt(color.slice(0, 2), 16),
    g: Number.parseInt(color.slice(2, 4), 16),
    b: Number.parseInt(color.slice(4, 6), 16)
  };
}

let nextRequestStartTime = 0;

async function waitForRequestSlot(args) {
  if (!args.requestSpacingMs) {
    return;
  }

  const now = Date.now();
  const startAt = Math.max(now, nextRequestStartTime);
  nextRequestStartTime = startAt + args.requestSpacingMs;
  const delay = startAt - now;
  if (delay > 0) {
    await sleep(delay);
  }
}

main().catch((error) => {
  console.error(error?.stack || error?.message || String(error));
  process.exit(1);
});
