const { XMLParser, XMLBuilder } = require("fast-xml-parser");
const fs = require('fs')


let SquadsFolder = './NPCs/Squads by difficulty/'

let parser = new XMLParser({
  ignoreAttributes: false,
  attributeNamePrefix: "@",
  commentPropName: '#',
  isArray: (name) => name == 'ItemSet',
});

let template = parser.parse(fs.readFileSync('./NPCs/template.xml', 'utf8'));

let npcIDs = template.npcsets.npcset.npc.map(npc => npc['@identifier'])
let itemSets = Object.fromEntries(npcIDs.map(id => [id, []]))

let squads = fs.readdirSync(SquadsFolder)
  .filter(name => name.endsWith('.xml'))
  .map(name => fs.readFileSync(SquadsFolder + name, 'utf8'))
  .map(raw => parser.parse(raw))
  .map(squad => squad.npcset.npc)


squads.forEach(squad => {
  squad.forEach(npc => {
    if (itemSets[npc['@identifier']]) {
      itemSets[npc['@identifier']] = itemSets[npc['@identifier']].concat(npc.ItemSet)
    } else {
      console.log(`id not found: ${npc['@identifier']}`);
    }
  })
})

template.npcsets.npcset.npc.forEach(npc => {
  npc.ItemSet = itemSets[npc['@identifier']]
})

let builder = new XMLBuilder({
  format: true,
  ignoreAttributes: false,
  attributeNamePrefix: "@",
  commentPropName: '#',
  suppressBooleanAttributes: false,
});


fs.writeFileSync('./Security.xml', builder.build(template))


