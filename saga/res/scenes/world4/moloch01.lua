local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()
local hero3 = getHero(2).getName()

walk('demon', 8, 'NORTH')

speak("Man", "You! Stop right there!")
speak(hero2, "Us?")
speak("Man", "Master Moloch knows you're here to find the pieces of EXCALIBER. Enemies of the masters must be destroyed!")
speak(hero1, "EXCALIBER?")
speak("Man", "Moloch and the prince will use EXCALIBER to smash the heart of Babel! And now for you!")

sceneSwitch('moloch01', true)
battle('encounter_lvl10_DEMON LORD', false)

wait(0.7)
speak(hero3, "Who is this Moloch?")
