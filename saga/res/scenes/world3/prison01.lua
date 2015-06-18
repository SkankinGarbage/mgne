local hero1 = getHero(0).getName()

speak("Man", "No way you can bend those bars. They're controlled electronically by the top guys that the prince trusts.")

wait(2.0)
playSound('explode')
shake(.3)
sceneSwitch('prison01', true)
wait(.5)
speak("Man", "Wow! Someone big must like you!")
speak(hero1, "...Thank you Janine.")
speak("Zkauba", "Hmph.")
