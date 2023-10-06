from mcje.minecraft import Minecraft
# import param_MCJE as param
import param_UNITY as param
from time import sleep

mc = Minecraft.create(port=param.PORT_MC)
mc.postToChat('set_columns2')

x = 0
z = -30
for _n in range(10):
    # y = param.Y_SEA + 1
    y = 0
    for _i in range(10):
        mc.setBlock(x, y, z,  param.SEA_LANTERN_BLOCK)
        sleep(0.2)
        y += 1
    z += 2
