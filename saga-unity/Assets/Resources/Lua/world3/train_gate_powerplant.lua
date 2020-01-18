speak('Conductor', 'I see you have a card. Would like to take the monorail to the power plant?')
choice()

fade('white')
wait(1.0)
teleport('world3/powerplant01.tmx', 14, 10, 'SOUTH', false)
fade('normal')
wait(1.0)
