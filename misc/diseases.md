{
  "name" : "",
  "type" : "",
  "level" : 1,
  "combine" : true,
  "duration" : 1,
  "vectors" : [],
  "chance" : 0.005,
  "effects" : [{"name": "", "level": , "start": , "end": }]
}
{
  "name" : "",
  "type" : "bacterial",
  "level" : 1,
  "combine" : true,
  "duration" : 1,
  "vectors" : [],
  "nextCond" : {},
  "chance" : 0.005,
  "effects" : [{"name": "", "level": , "start": , "end": }]
}
{
  "name" : "",
  "type" : "parasitic",
  "level" : 1,
  "combine" : true,
  "duration" : 1,
  "chance" : 0.005,
  "vectors" : [],
  "nextCond" : {
    "name" : "parasiticdamage",
    "type" : "scarring",
    "level" : 1,
    "combine" : true,
    "duration" : 1,
    "chance" : 1,
    "effects" : [{"name": "", "level": , "start": 0, "end": 1296}]
  },
  "effects" : [{"name": "", "level": , "start": , "end": }]
}
<strong>Endless Nightmare Syndrome</strong>
Corrupted/nightmare drifters

{
  "name" : "endlessns",
  "type" : "viral",
  "level" : 1,
  "combine" : true,
  "duration" : 1,
  "chance" : 0.01,
  "effects" : [{"name": "temporal-", "level": 3, "start": 72, "end": 432}]
},
{
  "name" : "endlessns",
  "type" : "viral",
  "level" : 2,
  "combine" : true,
  "duration" : 1,
  "chance" : 0.01,
  "effects" : [{"name": "temporal-", "level": 3, "start": 72, "end": 432}]
},
{
  "name" : "endlessns",
  "type" : "viral",
  "level" : 1,
  "combine" : true,
  "duration" : 2,
  "chance" : 0.01,
  "effects" : [{"name": "temporal-", "level": 3, "start": 72, "end": 432}]
}

<strong>Ravenous Beast Fever</strong>
Wolf bite; Rabbit Attack;

{
  "name" : "ravenousbf",
  "type" : "parasitic",
  "level" : 1,
  "duration" : 1,
  "combine" : true,
  "chance" : 0.005,
  "nextCond" : {
    "name" : "parasiticdamage",
    "type" : "scarring",
    "level" : 1,
    "combine" : true,
    "duration" : 1,
    "chance" : 1,
    "effects" : [{"name": "hunger+", "level": 3, "start": 0, "end": 1296}]
  },
  "vectors" : ["player", "wolf-*", "hare-*"],
  "effects" : [{"name": "hunger+", "level": 12, "start": 168, "end": 432},
   {"name": "punch+", "level": 4, "start": 168, "end": 432}]
}

<strong>Tetanus</strong>
All mechanical monster attacks

{
  "name" : "tetanus",
  "type" : "bacterial",
  "level" : 1,
  "combine" : true,
  "duration" : 1,
  "chance" : 0.01,
  "effects" : [{"name": "aim-", "level": 5, "start": 192, "end": 840},
  {"name": "mine-", "level": 5, "start": 192, "end": 840},
  {"name": "punch-", "level": 5, "start": 192, "end": 840},
  {"name": "speed-", "level": 7, "start": 216, "end": 840}
  {"name": "poison", "level": 500, "start": 839, "end": 840}]
}

<strong>Tetanus</strong>
A serious bacterial infection, stemming from open cuts from the mechanical monstrosities that lurk below, that causes the muscles to seize and lock up uncontrollably. This can severely impact an individual's quality of life making things like harvesting, mining, and even just a simple walk extremely difficult. If left untreated, can cause death.

{
  "name" : "tetanus",
  "type" : "bacterial",
  "level" : 1,
  "combine" : true,
  "duration" : 1,
  "chance" : 0.01,
  "effects" : [{"name": "aim-", "level": 5, "start": 192, "end": 840},
  {"name": "mine-", "level": 5, "start": 192, "end": 840},
  {"name": "punch-", "level": 5, "start": 192, "end": 840},
  {"name": "speed-", "level": 7, "start": 216, "end": 840}
  {"name": "poison", "level": 500, "start": 839, "end": 840}]
}

<strong>Pasteurella</strong>
Spontaneous
{
  "name" : "pasteurella",
  "type" : "bacterial",
  "level" : 2,
  "combine" : true,
  "duration" : 2,
  "chance" : 0.03,
  "nextCond" :{
    "name" : "pasteurella",
    "type" : "superbug",
    "level" : 1,
    "combine" : true,
    "duration" : 1,
    "chance" : 0.03,
    "vectors": ["hare-*", "pig-*", "chicken-*"],
    "effects" : [{"name": "spread+", "level": 5, "start": 24, "end": 336}, {"name": "immune-", "level": 5, "start": 24, "end": 336}]
  },
  "vectors" : ["hare-*", "pig-*", "chicken-*"],
  "effects" : [{"name": "spread+", "level": 5, "start": 24, "end": 336}, {"name": "immune-", "level": 5, "start": 24, "end": 336}]
}
<strong>Rabbit Fever</strong>
Spontaneous in rabbits; Rabbit attacks
{
  "name" : "tularemia",
  "type" : "bacterial",
  "level" : 1,
  "combine" : true,
  "duration" : 1,
  "vectors" : ["hare-*", "player"],
  "nextCond" : {
    "name" : "tularemia",
    "type" : "superbug",
    "level" : 1,
    "combine" : true,
    "duration" : 1,
    "vectors" : ["hare-*", "player"],
    "chance" : 0.01,
    "effects" : [{"name": "defense-", "level": 2, "start": 96, "end": 480}, {"name": "tolerate-", "level": 5, "start": 456, "end": 480}]
  },
  "chance" : 0.005,
  "effects" : [{"name": "defense-", "level": 2, "start": 96, "end": 480}, {"name": "tolerate-", "level": 5, "start": 456, "end": 480}]
}
<strong>Myxomatosis</strong>
Spontaneous in rabbits
{
  "name" : "myxomatosis,
  "type" : "viral",
  "level" : 1,
  "combine" : true,
  "duration" : 1,
  "vectors" : ["hare-*"],
  "chance" : 0.005,
  "effects" : [{"name": "defense-", "level": 3, "start": 72, "end": 288}]
},
{
  "name" : "myxomatosis,
  "type" : "viral",
  "level" : 2,
  "combine" : true,
  "duration" : 1,
  "vectors" : ["hare-*"],
  "chance" : 0.005,
  "effects" : [{"name": "defense-", "level": 3, "start": 72, "end": 288}]
},
{
  "name" : "myxomatosis,
  "type" : "viral",
  "level" : 1,
  "combine" : true,
  "duration" : 2,
  "vectors" : ["hare-*"],
  "chance" : 0.005,
  "effects" : [{"name": "defense-", "level": 3, "start": 72, "end": 288}]
}

<strong>Flystrike</strong>
Spontaneous in rabbits sheep chickens
{
  "name" : "flystrike",
  "type" : "parasitic",
  "level" : 1,
  "combine" : true,
  "duration" : 1,
  "chance" : 0.005,
  "vectors" : ["sheep-*", "hare-*", "chicken-*"],
  "nextCond" : {
    "name" : "parasiticdamage",
    "type" : "scarring",
    "level" : 1,
    "combine" : true,
    "duration" : 1,
    "chance" : 1,
    "effects" : [{"name": "coag-", "level": 2, "start": 0, "end": 1296}]
  },
  "effects" : [{"name": "bleed", "level": 1, "start": 24, "end": 48}, {"name": "coag-", "level": 4, "start": 24, "end": 48}]
}

<strong>Enzootic Pneumonia</strong>
Spontaneous in pigs
{
  "name" : "zoopneumonia",
  "type" : "viral",
  "level" : 1,
  "combine" : true,
  "duration" : 1,
  "vectors" : ["pig-*"],
  "chance" : 0.005,
  "effects" : [{"name": "", "level": , "start": , "end": }]
}

<strong>Swine Dysentery</strong>

<strong>Greasy Pig</strong>

<strong>Fowl Pox</strong>

<strong>Scabby Mouth</strong>

<strong>Q Fever</strong>

<strong>Coccidiosis</strong>
Poop
{
  "name" : "coccidiosis",
  "type" : "parasitic",
  "level" : 1,
  "combine" : true,
  "duration" : 1,
  "chance" : 0.01,
  "vectors" : ["hare-*", "pig-*", "chicken-*"],
  "nextCond" : {
    "name" : "parasiticdamage",
    "type" : "scarring",
    "level" : 1,
    "combine" : true,
    "duration" : 1,
    "chance" : 1,
    "effects" : [{"name": "hunger+", "level": 2, "start": 0, "end": 1296}]
  },
  "effects" : [{"name": "hunger+", "level": 4, "start": 144, "end": 384}, {"name": "bleed", "level": 4, "start": 144, "end": 148}, {"name": "bleed", "level": 4, "start": 200, "end": 204}, {"name": "bleed", "level": 4, "start": 380, "end": 384}]
}

<strong>Avian Flu</strong>
{
  "name" : "avianflu",
  "type" : "viral",
  "level" : 1,
  "combine" : true,
  "duration" : 1,
  "chance" : 0.01,
  contagiousCarrier: true,
  "vectors" : ["pig-*", "humanoid-*", "player"],
  "effects" : [{"name": "contagious", "level": 1, "start": 48, "end": 288}, {"name": "spread+", "level": 6, "start": 48, "end": 288}, {"name": "hunger+", "level": 2, "start": 48, "end": 288}]
},
{
  "name" : "avianflu",
  "type" : "viral",
  "level" : 1,
  "combine" : true,
  "duration" : 1,
  "chance" : 0.01,
  contagiousCarrier: true,
  "vectors" : ["pig-*", "humanoid-*", "player"],
  "effects" : [{"name": "contagious", "level": 1, "start": 48, "end": 288}, {"name": "spread+", "level": 6, "start": 48, "end": 288}, {"name": "hunger+", "level": 2, "start": 48, "end": 288}]
},
{
  "name" : "avianflu",
  "type" : "viral",
  "level" : 1,
  "combine" : true,
  "duration" : 2,
  "chance" : 0.01,
  contagiousCarrier: true,
  "vectors" : ["pig-*", "humanoid-*", "player"],
  "effects" : [{"name": "contagious", "level": 1, "start": 48, "end": 288}, {"name": "spread+", "level": 6, "start": 48, "end": 288}, {"name": "hunger+", "level": 2, "start": 48, "end": 288}]
}
<strong>Swine Flu</strong>
{
  "name" : "swineflu",
  "type" : "viral",
  "level" : 1,
  "combine" : true,
  "duration" : 1,
  "chance" : 0.01,
  contagiousCarrier: true,
  "vectors" : ["pig-*", "humanoid-*", "player"],
  "effects" : [{"name": "contagious", "level": 1, "start": 48, "end": 288}, {"name": "spread+", "level": 6, "start": 48, "end": 288}, {"name": "digest-", "level": 2, "start": 48, "end": 288}]
},
{
  "name" : "swineflu",
  "type" : "viral",
  "level" : 2,
  "combine" : true,
  "duration" : 1,
  "chance" : 0.01,
  contagiousCarrier: true,
  "vectors" : ["pig-*", "humanoid-*", "player"],
  "effects" : [{"name": "contagious", "level": 1, "start": 48, "end": 288}, {"name": "spread+", "level": 6, "start": 48, "end": 288}, {"name": "digest-", "level": 2, "start": 48, "end": 288}]
},
{
  "name" : "swineflu",
  "type" : "viral",
  "level" : 1,
  "combine" : true,
  "duration" : 2,
  "chance" : 0.01,
  contagiousCarrier: true,
  "vectors" : ["pig-*", "humanoid-*", "player"],
  "effects" : [{"name": "contagious", "level": 1, "start": 48, "end": 288}, {"name": "spread+", "level": 6, "start": 48, "end": 288}, {"name": "digest-", "level": 2, "start": 48, "end": 288}]
}
<strong>Lupinosis</strong>
{
  "name" : "lupinosis",
  "type" : "",
  "level" : 1,
  "combine" : true,
  "duration" : 1,
  "chance" : 0.01,
  "effects" : [{"name": "", "level": , "start": , "end": }]
}
<strong>Anthrax</strong>
{
  "name" : "anthrax",
  "type" : "bacterial",
  "level" : 1,
  "combine" : true,
  "duration" : 1,
  "chance" : 0.01,
  "effects" : [{"name": "", "level": , "start": , "end": }]
}
<strong>Typhoid Fever</strong>
{
  "name" : "salmonella",
  "type" : "bacterial",
  "level" : 1,
  "combine" : true,
  "duration" : 1,
  "chance" : 0.01,
  "effects" : [{"name": "", "level": , "start": , "end": }]
}
<strong>Malaria</strong>
breaking papyrus
{
  "name" : "malaria",
  "type" : "parasitic",
  "level" : 1,
  "combine" : true,
  "duration" : 1,
  "chance" : 0.01,
  "nextCond" : {
    "name" : "parasiticdamage",
    "type" : "scarring",
    "level" : 1,
    "combine" : true,
    "duration" : 1,
    "chance" : 1,
    "effects" : [{"name": "coag-", "level": 2, "start": 0, "end": 1296}]
  },
  "effects" : [{"name": "coag-", "level": 4, "start": 168, "end": 456}, {"name": "mine-", "level": 2, "start": 168, "end": 456}, {"name": "forage-", "level": 2, "start": 168, "end": 456}, {"name": "bow-", "level": 2, "start": 168, "end": 456}]
}

<strong>Dysentery</strong>
{
  "name" : "dysentery",
  "type" : "parasitic",
  "level" : 1,
  "combine" : true,
  "duration" : 1,
  "chance" : 0.01,
  "nextCond" : {
    "name" : "parasiticdamage",
    "type" : "scarring",
    "level" : 1,
    "combine" : true,
    "duration" : 1,
    "chance" : 1,
    "effects" : [{"name": "", "level": , "start": 0, "end": 1296}]
  },
  "effects" : [{"name": "", "level": , "start": , "end": }]
}
<strong>Leprosy</strong>
{
  "name" : "leprosy",
  "type" : "bacterial",
  "level" : 1,
  "combine" : true,
  "duration" : 1,
  "chance" : 0.01,
  "effects" : [{"name": "", "level": , "start": , "end": }]
}
<strong>Diphtheria</strong>
{
  "name" : "diphtheria",
  "type" : "bacterial",
  "level" : 1,
  "combine" : true,
  "duration" : 1,
  "chance" : ,
  "effects" : [{"name": "", "level": , "start": , "end": }]
}
<strong>Tuberculosis</strong>
{
  "name" : "tuberculosis",
  "type" : "bacterial",
  "level" : 1,
  "combine" : true,
  "duration" : 1,
  "chance" : 0.01,
  "effects" : [{"name": "", "level": , "start": , "end": }]
}
<strong>Cholera</strong>
{
  "name" : "cholera",
  "type" : "bacterial",
  "level" : 1,
  "combine" : true,
  "duration" : 1,
  "chance" : 0.01,
  "effects" : [{"name": "", "level": , "start": , "end": }]
}
<strong>The King's Evil</strong>
{
  "name" : "kingevil",
  "type" : "bacterial",
  "level" : 1,
  "combine" : true,
  "duration" : 1,
  "chance" : 0.01,
  "effects" : [{"name": "", "level": , "start": , "end": }]
}
<strong>Water Elf</strong>
{
  "name" : "waterelf",
  "type" : "viral",
  "level" : 1,
  "combine" : true,
  "duration" : 1,
  "chance" : 0.01,
  "effects" : [{"name": "", "level": , "start": , "end": }]
}
<strong>Black Death</strong>
{
  "name" : "blackdeath",
  "type" : "bacterial",
  "level" : 1,
  "combine" : true,
  "duration" : 1,
  "chance" : 0.01,
  "effects" : [{"name": "", "level": , "start": , "end": }]
}
<strong>Common Cold</strong>
{
  "name" : "cold",
  "type" : "viral",
  "level" : 1,
  "combine" : true,
  "duration" : 1,
  "chance" : 0.01,
  "effects" : [{"name": "", "level": , "start": , "end": }]
}
<strong>Aspergillosis</strong>
{
  "name" : "aspergillosis",
  "type" : "fungal",
  "level" : 1,
  "combine" : true,
  "duration" : 1,
  "chance" : 0.01,
  "effects" : [{"name": "", "level": , "start": , "end": }]
}
<strong>Smallpox</strong>
{
  "name" : "smallpox",
  "type" : "viral",
  "level" : 1,
  "combine" : true,
  "duration" : 1,
  "chance" : 0.01,
  "effects" : [{"name": "", "level": , "start": , "end": }]
}
<strong>Guinea Worm Disease</strong>
Breaking papyrus
{
  "name" : "guineaworm",
  "type" : "parasitic",
  "level" : 1,
  "combine" : true,
  "duration" : 1,
  "chance" : 0.01,
  "nextCond" : {
    "name" : "parasiticdamage",
    "type" : "scarring",
    "level" : 1,
    "combine" : true,
    "duration" : 1,
    "chance" : 1,
    "effects" : [{"name": "speed-", "level": 2, "start": 0, "end": 1296}]
  },
  "effects" : [{"name": "speed-", "level": 4, "start": 216, "end": 288}, {"name": "immune-", "level": 6, "start": 240, "end": 288}]
}
