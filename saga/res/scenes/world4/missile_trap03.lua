walk('guard_l3', 'WEST', 1, false)
walk('guard_r3', 'EAST', 1)
face('guard_l3', 'NORTH')
face('guard_r3', 'NORTH')

sceneSwitch('missile_trap03_01', true)
shake(0.5, 4)
wait(0.5)
sceneSwitch('missile_trap03_02', true)
shake(0.5, 4)
wait(0.5)

sceneSwitch('missile_trap03', true)
