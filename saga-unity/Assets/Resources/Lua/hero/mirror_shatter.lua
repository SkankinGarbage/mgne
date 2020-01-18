local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()
local hero3 = getHero(2).getName()
local hero4 = getHero(3).getName()

speak(hero3, "Is this the mirror?")
speak(hero2, "Should we really break this...?")
speak(hero1, "We need to find Jonas and Janine. If this is the only way...")

wait(0.4)
playSound('rebound')
wait(0.8)

speak(hero4, "It is done.")
speak(hero1, "Let's see if the gate has changed.")
sceneSwitch('mirror_shatter', true)