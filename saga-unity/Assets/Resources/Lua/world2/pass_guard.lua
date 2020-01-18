local hero1 = getHero(1).getName()

speak("Guard", "Are you a soldier of the wolf?")
speak(hero1, "Well...")
speak("Guard", "Then stay out!")
pathEvent('hero', 'target')
face('hero', 'SOUTH')
