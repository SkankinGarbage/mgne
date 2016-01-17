local hero1 = getHero(0).getName()

speak("Zkauba", "Hrrrrgh!")
speak("Man", "No way you can bend those bars.")
wait(2.0)

walk('janine', 2, 'SOUTH')
walk('janine', 2, 'EAST')
face('janine', 'SOUTH')
wait(0.8)
walk('janine', 1, 'EAST')
wait(0.4)

playSound('explode')
shake(0.3)
sceneSwitch('prison_lever', true)
wait(0.5)

walk('janine', 3, 'WEST')
walk('janine', 2, 'NORTH')

speak("Man", "Wow! Who was that?")
speak(hero1, "...Thank you Janine.")
speak("Zkauba", "Hmph.")

sceneSwitch('prison01', true)
