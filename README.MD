-----Current Status Effects-----
Got pretty lazy with the names past this point. They are all simply [statuseffectname]+ for a positive effect or [statuseffectname]- for a negative effect, for example "speed+" increases walk speed, "speed-" decreases walk speed. Only effects with +/- work like that.

"regen" = Regenerate 5% of your current max health on update per level
"poison" = Take 5% damage every up is increased or decreased by poison tolerance per level
"bleed" = Take 5% damage, divided by healing rate per level
"speed+/-" = 5% increase/decrease in move speed  per level
"health+/-" = 5% more/less max health per level
"coag+/-" = 5% more/less healing rate per level
"tolerate+/-" = 5% more/less poison resistance per level
"hunger+/-" = 5% more/less hunger rate per level
"aim+/-" = 5% more/less aiming speed per level
"acc+/-" = 5% more/less aiming accuracy
"bow+/-" = 5% more/less throw velocity
"revive" = Revives entity when dies, level irrelevant
"temporal+/-" = 5% temporal stability gain/loss per update per level
"metabolism+/-" = Decreases/increases update time by 6 (in game) minutes per level
"absorb" = Gain 1% health from attacking per level
"defense+/-" = Take 5% less/more damage per level
"resistance" = Gain a resistance temporarily(need to fill in name of resistance in addInfo)
"immunity" = Gain a resistance temporarily(need to fill in name of resistance in addInfo)
"contagious" = Makes disease contagious
"spread+/-" = Increases/decreases contagious spread range per level
"immune+/-" = Increases/decreases immune system strength by 5% per level
"revenge" = Explode on death, range increased by level
"recoil" = Reflects 5% of damage to attacker per level
"hide" = Decreases animal seeking range by 5% per level
"immortal" = Stop taking damage after reaching a health threshold. 5% higher threshold
"punch+/-" = Deal 5% more/less melee damage per level
"throw+/-" = Deal 5% more/less ranged damage per levels
"digest+/-" = Get 5% more/less saturation from food

Effects that only affect the player past this point
"gear+/-" = Increases/decreases rusty gear collection chance by 5% per level
"mine+/-" : Increases/decreases mining speed by 5% per level
"forage+/-" = Increases/decreases foraging abilities by 5% per level
"crop+/-" = Increases/decreases wild crop harvesting abilities by 5% per level
"hunt+/-" = Increases/decreases animal harvest drops and animal harvesting speed by 5% per level
"ore+/-" = Increase/decreases ore drop rate by 5% per level
"armor+/-" = Increases/decreases armor durability  by 5% per level
"vessel+/-" : Increases/decreases vessel drop rate


-----Condition Structure-----
name = string Name of the condition
level = int Level of the condition, multiplies children effect levels
duration = int Duration of condition, multiplies children effect durations
effect = [] Array of effects
mech = bool Whether this effects mechanical creatures or not
type = string Type of condition (eg. "bacterial")
chance = decimal between 0-1, Determines chance for disease to be applied
combine = bool Whether the condition will combine with an existing condition
contagiousChance = decimal between 0-1, Determines chance for disease to be spread
contagiousCarrier = bool Whether or not a carrier of this condition will spread it
vectors = string[] What entity types that this disease can spread to if contagious
nextCond = Condition A condition to be applied after current one ends (laceration => infection)
nextCondChance = decimal between 0-1, Additional chance multiplier for next condition

-----Effect Structure-----
name = string name of effect (See Currrent Status Effects)
level = int Level of the effect
start = int When the effect starts
end = int When effect ends
addInfo = string Used to store information for resistance and immunity effect

-----Item Attributes-----
For medicine:
herbProperties = Array of Conditions/Diseases, can be placed in bottle to make medicine
firstAidRemoval = Array of Disease names to be cured when applied, must be use with ItemFirstAid in order to work,

To make an augment:
attackInjuries = Array of Conditions/Diseases, can be used to augment an item with the ItemEnhancedWeapon class
augmentTime = An integer used by weapon augments to determine how long it lasts
augmentName = A string of the augment code for the name

To make a poison:
arrowPoison = Array of Conditions/Diseases, can be used to apply conditions to arrows
poisonName = A string for the poison code for the name

Squeezable for Press:
squeezeInto = Domain + code of liquid to turn into
squeezeRatio = The number of items it takes to make 1 portion

-----Conds Controller Behavior Attributes-----
spontaneousDiseases = An array of conditions that can occur spontaneously
attackDiseases = An array of conditions that the entity can pass on when attacking
defenseless = A bool that will reverse attack diseases so they are transmitted when the entity is attacked
carrierOf = An array of strings which are the names of conditions this entity can be an asymptomatic carrier of

  -----Conds Controller Amounts-----
  These amounts act as defaults for the entity. You can either put an integer, which will act as a static default, or an array of two integers which will then randomly choose a number between them for each entity
  poisonAmount = Default poison damage taken every update (Negative numbers = 0)
  bleedAmount = Default bleed damage taken every update (Negative numbers = 0)
  regenAmount = Default healing every update (Negative numbers = 0)
  speedAmount = Default speed modifier of the entity
  healthAmount = Default max health modifier of the entity
  coagAmount = Default healing effectiveness modifier
  hungerAmount = Default hunger rate modifier
  aimAmount = Default aim speed modifier
  tolerateAmount = Default poison tolerance modifier
  reviveAmount = Default revive modifier. (After a death and revive this is always set to zero, so any amount greater than 1 will not result in more revives)
  temporalAmount = Default temporal gain/lost per update
  absorbAmount = Default damage absorption amount (Negative numbers = 0)
  defenseAmount = Default damage reduction modifier
  attackAmount = Default attack damage modifier
  immuneAmount = Default immune system protection
  contagiousAmount = Default contagious range
  recoilAmount = Default recoil damage modifier (Negative numbers = 0)
  revengeAmount = Default explosion on death modifier  (Negative numbers = 0)
  speed = Default condition update/metabolism speed.


