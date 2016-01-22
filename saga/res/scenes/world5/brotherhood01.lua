local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()

sceneSwitch('brotherhood_appears', true)

walk('zkauba', 10, 'WEST', false)
walk('lara', 10, 'WEST', true)
walk('zkauba', 2, 'WEST', false)
walk('lara', 2, 'NORTH', true)
walk('zkauba', 1, 'NORTH', true)

speak(hero2, "Look, it's Zkauba and Lara!")
speak("Lara", hero1 .. ", you're -")
speak("Zkauba", "We were worried! Jonas dead, the door to Babel's heartland opened...")
speak("Janine", "Jonas...")
speak("Lara", "We buried him in the foothills. Say, aren't you -")
speak("Zkauba", "His treacherous twin! You're lucky I don't kill you where you stand!")
speak(hero1, "Jonas died to protect her.")
speak("Janine", "And I'll avenge him. Aven betrayed us.")
speak("Lara", "We thought so... He tried to lose us in Babel, and -")
speak("Zkauba", "And we almost died trying to find a way out. We'll try to help you if we still can.")
speak("Lara", "Aven may have left the brotherhood but we'll still fight for its ideals. No more masters!")

walk('lara', 2, 'NORTH', false)
walk('zkauba', 2, 'NORTH', true)

sceneSwitch('brotherhood_appears', false)
sceneSwitch('brotherhood01', true)
