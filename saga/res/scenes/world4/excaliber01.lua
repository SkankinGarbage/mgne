local hero1 = getHero(0).getName()

speak("Guard", "Who are you?")
speak(hero1, "We're from Babel. Do you guard EXCALIBER?")
speak("Guard", "EXCALIBER needs no guard. The sword is broken. But maybe if you're from Babel...")

walk('guard', 1, 'NORTH')
walk('guard', 1, 'WEST')
face('guard', 'EAST')

speak("Guard", "We've guard this piece too long. Maybe you can use EXCALIBER to slay the evil Moloch.")
sceneSwitch('excaliber01')