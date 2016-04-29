local hero1 = getHero(0).getName()

speak("Guard", "Who are you?")
speak(hero1, "We're from Babel. Do you guard EXCALIBUR?")
speak("Guard", "EXCALIBUR needs no guard. The sword is broken. But maybe if you're from Babel...")

walk('guard', 1, 'NORTH')
walk('guard', 1, 'WEST')
face('guard', 'EAST')

speak("Guard", "We've kept this piece too long. Maybe you're the one to reforge EXCALIBUR.")
sceneSwitch('excaliber01', true)
