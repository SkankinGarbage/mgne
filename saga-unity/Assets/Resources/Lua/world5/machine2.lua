sceneSwitch('machine_chest', true)
playSound('get')
speak("Retrieved the $CSPEED.")
addItem('hand_speed')
wait(0.7)
playBGM('elemental_boss')
speak("????", "INTRUDER ALERT. PREPARE THE SECURITY SYSTEM!!")
sceneSwitch('machine2_dead', true)
battle('party_bossMachine2', true, 'elemental_boss')
playBGM('ffl3_dungeon')