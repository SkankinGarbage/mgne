walk('guard_l3', 2, 'EAST', false)
walk('guard_r3', 2, 'WEST')
face('guard_l3', 'NORTH')
face('guard_r3', 'NORTH')

playSound('explode')
sceneSwitch('missile_trap03_01', true)
shake(0.5, 4)
wait(0.5)
playSound('explode')
sceneSwitch('missile_trap03_02', true)
shake(0.5, 4)
wait(0.5)

sceneSwitch('missile_trap03', true)
