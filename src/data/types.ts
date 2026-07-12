export type SchoolId =
  | "arcane"
  | "fire"
  | "cold"
  | "lightning"
  | "earth"
  | "mind"
  | "blood"
  | "poison";

export type UpgradePath = {
  name: string;
  summary: string;
};

export type Skill = {
  name: string;
  role: string;
  upgradePaths: [UpgradePath, UpgradePath, UpgradePath];
};

export type SchoolIdentity = {
  baseDamage: string;
  castStyle: string;
  areaShape: string;
  range: string;
  cooldown: string;
  proc1: string;
  proc2: string;
};

export type SchoolDefinition = {
  id: SchoolId;
  name: string;
  role: string;
  identity: SchoolIdentity;
  valueProposition: string;
  skills: [Skill, Skill, Skill, Skill, Skill];
};
