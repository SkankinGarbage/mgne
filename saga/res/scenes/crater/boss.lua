local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()
local hero3 = getHero(2).getName()
local hero4 = getHero(3).getName()

playSound('get')
addItem('key_excaliber3')
sceneSwitch('crater_chest', true)
speak("Retrieved the EXCALIBER3.")
wait(0.8)

speak(hero2, "Part of EXCALIBER? Is this a fake?")
speak("????", "Afraid not!")

sceneSwitch('moloch_appears', true)
face('hero', 'NORTH')
walk('moloch', 7, 'SOUTH')

speak(hero4, "Moloch!")
speak("Moloch", "Your idiot friend ran off with the fake. But this one's real.")
speak("Moloch", "And I think you've delivered me the other two pieces. Will you hand them over?")
speak(hero1, "Never!")
speak("Moloch", "One last chance... Give me the shards and I let you live. Or don't you fear the masters?")
speak(hero1, "We'll kill you with or without EXCALIBER!")

sceneSwitch('moloch_appears', false)
battle('party_bossMoloch', false, 'ffl1_boss')
face('hero', 'NORTH')
playBGM('ffl3_ruins')

wait(0.7)
speak(hero2, "Phew... Now we can reforge EXCALIBER at Elan!")
speak(hero1, "We should find Jonas.")
sceneSwitch('world4_done', true)
