local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()
local hero3 = getHero(2).getName()

speak('Man', "You... Did you come from Babel?")
speak(hero2, "Babel?")
speak('Man', "The world beyond the gate. That's its name.")
speak(hero1, "Are you alright?")
speak('Man', "I will not live long. But if you are from Babel you must know...")
speak('Man', "The tower, built in ancient times to reach the world of the gods...")
speak('Man', "Babel's gates open again... Gods walk among us!")
speak(hero1, "Please, calm down!")
speak('Man', "Good luck.")

wait(1.0)
sceneSwitch('f2_sideworld', true)
wait(0.8)

speak(hero3, "Goodbye, old man.")
wait(0.8)

speak(hero2, "Babel? What is this nonsense about gods?")
speak(hero1, "Well, if Jonas and Janine made it this far, they'll know. I hope we're on the right track.")
