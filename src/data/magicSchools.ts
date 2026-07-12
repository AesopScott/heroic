import type { SchoolDefinition } from "./types";

export const magicSchools: SchoolDefinition[] = [
  {
    id: "arcane",
    name: "Arcane",
    role: "repeat casting, consistency, double-cast synergy",
    identity: {
      baseDamage: "Low",
      castStyle: "Instant",
      areaShape: "Limited",
      range: "Long",
      cooldown: "Short",
      proc1: "Double Cast",
      proc2: "None",
    },
    valueProposition:
      "Arcane is the consistency school. It rewards frequent casting, repeat effects, and safe long-range play.",
    skills: [
      {
        name: "Magic Missile",
        role: "starter attack, reliable single-target pressure",
        upgradePaths: [
          { name: "Split Shot", summary: "More missiles and better crowd coverage." },
          { name: "Seeking Shot", summary: "Stronger homing and better target retention." },
          { name: "Arcane Pierce", summary: "Missiles pass through enemies for line pressure." },
        ],
      },
      {
        name: "Arcane Blast",
        role: "direct burst damage",
        upgradePaths: [
          { name: "Power", summary: "Higher damage for elites and bosses." },
          { name: "Reach", summary: "Longer range and safer positioning." },
          { name: "Scatter", summary: "Hits additional nearby targets." },
        ],
      },
      {
        name: "Warp Pulse",
        role: "control, spacing, disruption",
        upgradePaths: [
          { name: "Push", summary: "Knocks enemies away to create breathing room." },
          { name: "Pull", summary: "Draws enemies inward for combo setups." },
          { name: "Slow Warp", summary: "Applies a movement slow to hit enemies." },
        ],
      },
      {
        name: "Spell Echo",
        role: "repeat-cast enhancer",
        upgradePaths: [
          { name: "Repeat", summary: "The spell repeats after a delay." },
          { name: "Amplify", summary: "The echo is stronger than the original." },
          { name: "Chain Echo", summary: "The echo jumps to another target or zone." },
        ],
      },
      {
        name: "Arcane Orbit",
        role: "defensive offense, constant pressure",
        upgradePaths: [
          { name: "More Orbs", summary: "Additional orbitals for more contact damage." },
          { name: "Faster Orbs", summary: "Higher hit frequency from quicker rotation." },
          { name: "Larger Orbs", summary: "Bigger hit radius and better safety." },
        ],
      },
    ],
  },
  {
    id: "fire",
    name: "Fire",
    role: "burst damage, area denial, burn pressure",
    identity: {
      baseDamage: "Very High",
      castStyle: "Standard, 1 sec",
      areaShape: "Many",
      range: "Standard",
      cooldown: "Standard",
      proc1: "Burn",
      proc2: "Burning Area",
    },
    valueProposition:
      "Fire is the destruction school. It rewards immediate impact, crowd clearing, and battlefield denial.",
    skills: [
      {
        name: "Fireball",
        role: "core burst attack",
        upgradePaths: [
          { name: "Impact", summary: "Higher direct hit damage." },
          { name: "Explosion", summary: "Bigger blast radius for groups." },
          { name: "Burn", summary: "Stronger damage over time." },
        ],
      },
      {
        name: "Flame Wave",
        role: "area sweep and crowd clear",
        upgradePaths: [
          { name: "Wider Wave", summary: "Covers more space." },
          { name: "Longer Wave", summary: "Travels farther across the arena." },
          { name: "Hotter Wave", summary: "Higher tick damage over time." },
        ],
      },
      {
        name: "Ember Rain",
        role: "delayed burst and zone pressure",
        upgradePaths: [
          { name: "More Meteors", summary: "Extra falling hits for swarm clear." },
          { name: "Faster Rain", summary: "Shorter delay between strikes." },
          { name: "Firestorm", summary: "Leaves burning ground behind." },
        ],
      },
      {
        name: "Ignition",
        role: "burn amplifier",
        upgradePaths: [
          { name: "Spread", summary: "Burn jumps to nearby enemies." },
          { name: "Intensify", summary: "Burn stacks harder." },
          { name: "Detonate", summary: "Burning enemies explode on death." },
        ],
      },
      {
        name: "Cinder Wall",
        role: "area denial and choke control",
        upgradePaths: [
          { name: "Longer Wall", summary: "More coverage for lanes and choke points." },
          { name: "Hotter Wall", summary: "More damage on contact." },
          { name: "Moving Wall", summary: "The wall slowly advances." },
        ],
      },
    ],
  },
  {
    id: "cold",
    name: "Cold",
    role: "control, slow, freeze, spacing",
    identity: {
      baseDamage: "Standard",
      castStyle: "Standard, 1 sec",
      areaShape: "Many",
      range: "Short",
      cooldown: "Slow",
      proc1: "Slow",
      proc2: "Freeze",
    },
    valueProposition:
      "Cold is the control school. It wins by controlling space, reducing enemy speed, and setting up freeze windows.",
    skills: [
      {
        name: "Frost Ring",
        role: "close-range area control",
        upgradePaths: [
          { name: "Wider Ring", summary: "Larger radius for better swarm coverage." },
          { name: "Heavier Chill", summary: "Stronger slow and lock-down play." },
          { name: "Deep Freeze", summary: "Higher chance to freeze enemies." },
        ],
      },
      {
        name: "Ice Shard",
        role: "single-target pressure with crowd overlap",
        upgradePaths: [
          { name: "More Shards", summary: "More projectiles for wider coverage." },
          { name: "Piercing Shards", summary: "Shards pass through enemies." },
          { name: "Shatter Damage", summary: "Bonus damage to slowed or frozen targets." },
        ],
      },
      {
        name: "Glacial Field",
        role: "battlefield control",
        upgradePaths: [
          { name: "Wider Field", summary: "Bigger zone for stronger denial." },
          { name: "Longer Field", summary: "Lasts longer for sustained control." },
          { name: "Deeper Chill", summary: "Stronger slow and attack slowdown." },
        ],
      },
      {
        name: "Crystal Prison",
        role: "hard control and trap setting",
        upgradePaths: [
          { name: "More Prisons", summary: "More ice traps for better coverage." },
          { name: "Faster Trigger", summary: "Traps activate sooner." },
          { name: "Hard Lock", summary: "Stronger freeze or root for elites." },
        ],
      },
      {
        name: "Shatter Line",
        role: "lane clear and freeze burst",
        upgradePaths: [
          { name: "Wider Line", summary: "Easier to catch enemies." },
          { name: "Longer Line", summary: "More reach for lane shaping." },
          { name: "Brutal Shatter", summary: "Bonus damage to chilled or frozen targets." },
        ],
      },
    ],
  },
  {
    id: "lightning",
    name: "Lightning",
    role: "fast burst, chain damage, stun pressure",
    identity: {
      baseDamage: "High",
      castStyle: "Fast, 0.5 sec",
      areaShape: "Linear + Chains",
      range: "Long",
      cooldown: "Standard",
      proc1: "Stun",
      proc2: "None",
    },
    valueProposition:
      "Lightning is the tempo school. It wins by reacting quickly, chaining damage through groups, and interrupting momentum.",
    skills: [
      {
        name: "Chain Bolt",
        role: "core chain damage",
        upgradePaths: [
          { name: "More Jumps", summary: "Hits more enemies." },
          { name: "Higher Damage", summary: "Stronger per-hit burst." },
          { name: "Longer Chain", summary: "Leaps farther between targets." },
        ],
      },
      {
        name: "Static Field",
        role: "area control and stun setup",
        upgradePaths: [
          { name: "Bigger Field", summary: "Larger zone coverage." },
          { name: "Faster Ticks", summary: "More frequent damage pressure." },
          { name: "Stun Chance", summary: "Briefly stuns enemies." },
        ],
      },
      {
        name: "Thunder Lance",
        role: "line burst and priority target damage",
        upgradePaths: [
          { name: "Piercing Lance", summary: "Passes through more enemies." },
          { name: "Wider Lance", summary: "Easier to hit packed groups." },
          { name: "Critical Strike", summary: "Higher burst against isolated targets." },
        ],
      },
      {
        name: "Spark Surge",
        role: "burst tempo and light crowd pressure",
        upgradePaths: [
          { name: "More Sparks", summary: "Extra bolts for multi-target value." },
          { name: "Faster Surge", summary: "Shorter burst window." },
          { name: "Target Spread", summary: "Sparks arc to nearby enemies." },
        ],
      },
      {
        name: "Storm Call",
        role: "high-end area burst and stun pressure",
        upgradePaths: [
          { name: "More Strikes", summary: "Additional hits for swarm clear." },
          { name: "Faster Strikes", summary: "Shorter delay between hits." },
          { name: "Violent Storm", summary: "Stronger damage and stun potential." },
        ],
      },
    ],
  },
  {
    id: "earth",
    name: "Earth",
    role: "heavy disruption, terrain control, long-range impact",
    identity: {
      baseDamage: "Standard",
      castStyle: "Slow, 2 sec",
      areaShape: "Large Areas",
      range: "Very Long",
      cooldown: "Slow",
      proc1: "Knockdown",
      proc2: "Stun",
    },
    valueProposition:
      "Earth is the heavy control school. It shapes the battlefield, forces bad movement, and lands large disruptive hits from far away.",
    skills: [
      {
        name: "Stone Spike",
        role: "reliable terrain strike",
        upgradePaths: [
          { name: "More Spikes", summary: "Extra spike hits for better swarm coverage." },
          { name: "Larger Spikes", summary: "Stronger damage per hit." },
          { name: "Ground Breaker", summary: "Spikes also disturb terrain." },
        ],
      },
      {
        name: "Boulder Toss",
        role: "impact damage and disruption",
        upgradePaths: [
          { name: "Bigger Boulder", summary: "More damage on impact." },
          { name: "More Bounce", summary: "Continues into more targets." },
          { name: "Crushing Boulder", summary: "Stronger knockback and stun value." },
        ],
      },
      {
        name: "Earth Wall",
        role: "battlefield shaping",
        upgradePaths: [
          { name: "Longer Wall", summary: "More coverage for lanes and choke points." },
          { name: "Taller Wall", summary: "Blocks more movement paths." },
          { name: "Harden Wall", summary: "Lasts longer and resists damage." },
        ],
      },
      {
        name: "Quake",
        role: "large-area disruption",
        upgradePaths: [
          { name: "Larger Quake", summary: "Wider radius for better coverage." },
          { name: "Stronger Quake", summary: "More damage." },
          { name: "Repeated Quake", summary: "Multiple pulses over time." },
        ],
      },
      {
        name: "Mud Trap",
        role: "slow field and movement control",
        upgradePaths: [
          { name: "Bigger Trap", summary: "Larger slow zone." },
          { name: "Stickier Mud", summary: "Stronger slow." },
          { name: "Heavy Mud", summary: "Enemies in it take extra damage." },
        ],
      },
    ],
  },
  {
    id: "mind",
    name: "Mind",
    role: "fear, confusion, decoys, behavior disruption",
    identity: {
      baseDamage: "Standard",
      castStyle: "Instant",
      areaShape: "Cones",
      range: "Short",
      cooldown: "Short",
      proc1: "Fear",
      proc2: "Confuse",
    },
    valueProposition:
      "Mind is the disruption school. It breaks enemy behavior, forces bad movement, and turns pressure into chaos.",
    skills: [
      {
        name: "Psychic Lance",
        role: "precision disruption",
        upgradePaths: [
          { name: "More Damage", summary: "Stronger hit for elites." },
          { name: "Longer Range", summary: "Safer casting distance." },
          { name: "Mind Pierce", summary: "Ignores part of enemy defenses." },
        ],
      },
      {
        name: "Fear Wave",
        role: "crowd scatter and spacing",
        upgradePaths: [
          { name: "Bigger Wave", summary: "Wider cone and better reach." },
          { name: "Longer Fear", summary: "Enemies flee for more time." },
          { name: "Stronger Panic", summary: "Enemies behave less predictably." },
        ],
      },
      {
        name: "Illusion Clone",
        role: "decoy and positioning support",
        upgradePaths: [
          { name: "More Clones", summary: "Extra decoys for better distraction." },
          { name: "Stronger Decoys", summary: "Clones last longer." },
          { name: "Clone Burst", summary: "Clones punish nearby enemies when destroyed." },
        ],
      },
      {
        name: "Confuse",
        role: "behavior disruption",
        upgradePaths: [
          { name: "Wider Effect", summary: "Affects more enemies." },
          { name: "Longer Confusion", summary: "Lasts longer." },
          { name: "Deeper Confusion", summary: "Enemies turn on each other more often." },
        ],
      },
      {
        name: "Mind Crush",
        role: "finisher and burst disruption",
        upgradePaths: [
          { name: "More Damage", summary: "Stronger burst." },
          { name: "Area Crush", summary: "Hits a wider zone." },
          { name: "Execution Crush", summary: "Does more damage to weakened enemies." },
        ],
      },
    ],
  },
  {
    id: "blood",
    name: "Blood",
    role: "sacrifice, bleed, drain, risky sustain",
    identity: {
      baseDamage: "Standard",
      castStyle: "Standard",
      areaShape: "Ground Effects and single targets",
      range: "Standard",
      cooldown: "Standard",
      proc1: "Bleed",
      proc2: "Drain",
    },
    valueProposition:
      "Blood is the sacrifice school. It trades life for power, drains enemies to stay alive, and applies relentless bleed pressure.",
    skills: [
      {
        name: "Blood Bolt",
        role: "reliable sustain damage",
        upgradePaths: [
          { name: "More Damage", summary: "Stronger direct hit." },
          { name: "Lifesteal", summary: "Heal from hits." },
          { name: "Splash Drain", summary: "Damages and drains nearby enemies." },
        ],
      },
      {
        name: "Sanguine Pact",
        role: "sacrifice engine",
        upgradePaths: [
          { name: "More Power", summary: "Bigger gain from sacrifice." },
          { name: "More Healing", summary: "Better recovery after sacrifice." },
          { name: "Lower Cost", summary: "Sacrifice costs less health." },
        ],
      },
      {
        name: "Blood Nova",
        role: "burst and sustain",
        upgradePaths: [
          { name: "Bigger Nova", summary: "Larger blast radius." },
          { name: "Stronger Nova", summary: "More damage." },
          { name: "Healing Nova", summary: "Damages enemies and heals the caster." },
        ],
      },
      {
        name: "Leech Bind",
        role: "sustain lock and attrition",
        upgradePaths: [
          { name: "Longer Bind", summary: "Lasts longer for more drain time." },
          { name: "Stronger Drain", summary: "More life stolen." },
          { name: "Multi-Bind", summary: "Links multiple enemies together." },
        ],
      },
      {
        name: "Crimson Frenzy",
        role: "aggressive self-buff",
        upgradePaths: [
          { name: "Faster Attacks", summary: "More attack speed." },
          { name: "More Damage", summary: "Stronger offensive output." },
          { name: "Low Health Power", summary: "Stronger when near death." },
        ],
      },
    ],
  },
  {
    id: "poison",
    name: "Poison",
    role: "damage over time, spread, contamination, delayed collapse",
    identity: {
      baseDamage: "High",
      castStyle: "Standard DoTs",
      areaShape: "Many",
      range: "Standard",
      cooldown: "Long",
      proc1: "Contagious",
      proc2: "Disabled",
    },
    valueProposition:
      "Poison is the attrition school. It spreads damage over time, weakens groups, and collapses enemy formations through infection.",
    skills: [
      {
        name: "Poison Dart",
        role: "core poison application",
        upgradePaths: [
          { name: "More Darts", summary: "Extra shots for better coverage." },
          { name: "Stronger Poison", summary: "More damage over time." },
          { name: "Spread Poison", summary: "Poison jumps on hit." },
        ],
      },
      {
        name: "Toxic Cloud",
        role: "area denial and attrition",
        upgradePaths: [
          { name: "Bigger Cloud", summary: "Larger area for more coverage." },
          { name: "Longer Cloud", summary: "Cloud lasts longer." },
          { name: "Heavier Cloud", summary: "Stronger damage over time." },
        ],
      },
      {
        name: "Venom Trail",
        role: "path control and pursuit pressure",
        upgradePaths: [
          { name: "Longer Trail", summary: "More coverage." },
          { name: "Stronger Trail", summary: "More damage." },
          { name: "Sticky Trail", summary: "Enemies are slowed in it." },
        ],
      },
      {
        name: "Infection",
        role: "contagion and snowball damage",
        upgradePaths: [
          { name: "Faster Spread", summary: "Poison jumps more often." },
          { name: "Stronger Infection", summary: "Damage ramps harder." },
          { name: "Collapse", summary: "Infected enemies burst on death." },
        ],
      },
      {
        name: "Rot Bloom",
        role: "burst into lingering area",
        upgradePaths: [
          { name: "Bigger Bloom", summary: "Larger explosion." },
          { name: "More Bloom Damage", summary: "Stronger burst." },
          { name: "Lingering Rot", summary: "Leaves a poisonous zone behind." },
        ],
      },
    ],
  },
];
