local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()

speak("Seer", "Welcome back, " .. hero1 .. ".")
speak(hero1, "Moloch is dead!")
speak("Seer", "I knew you could do it. But you didn't have EXCALIBER?")
speak(hero2, "We found the third piece in his tower. Can you reassemble the sword?")
speak("Seer", "Come to the forge.")

fade('black')
wait(1.0)
sceneSwitch('elan_elder03', true)
teleport('world4/elan_cave.tmx', 38, 16, 'WEST', false)
wait(1.0)
fade('normal')
wait(2.0)

face('smithy', 'SOUTH')
wait(0.3)
speak("Seer", "Take your prize.")
wait(0.1)
playBGM('ffl1_get')
speak("Retrieved the $XEXCALIBER.")
wait(3.5)
playBGM('ffl1_town')
speak(hero1, "EXCALIBER is whole again.")
speak("Seer", "Now go to Babel. The masters must not touch Babel's heart!")

removeCollectable('collectable_excaliber', true)
removeCollectable('collectable_excaliber', true)
removeCollectable('collectable_excaliber', true)
addItem('wpn_legendExcaliber')

sceneSwitch('got_excaliber', true)
