local hero1 = getHero(0).getName()
local hero2 = getHero(3).getName()

speak(hero2, "Is that all of them?")
speak(hero1, "Let's report back to Aven.")

teleport('world3/overworld.tmx', 32, 14)
sceneSwitch('mines_done', true)
