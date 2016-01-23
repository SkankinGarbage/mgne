local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()

playSound('explode')
shake(0.5)
wait(0.3)
playSound('roar')
shake(0.4)
wait(0.8)

speak(hero2, "What was that?")

walk('zkauba', 7, 'EAST', false)
walk('lara', 7, 'EAST', true)
face('hero', 'WEST')

speak(hero1, "Zkauba? Lara?")
speak("Lara", "We rushed to get here. We were at the gate and -")
speak("Zkauba", "Babel is starting to get unstable! It sounds like something is happening up at the palace.")
speak("Lara", "It must be Aven's doing...")
speak(hero2, "Babel is unstable?")
speak("Lara", "I don't know, it feels like many seals are being loosened at once. Like something big is going to happen.")
speak("Zkauba", "Go stop Aven before it happens!")
speak("Lara", "There's a lot riding on you. Give it your best!")

face('hero', 'SOUTH')
walk('zkauba', 9, 'SOUTH', false)
walk('lara', 9, 'SOUTH', true)

sceneSwitch('brotherhood02', true)
