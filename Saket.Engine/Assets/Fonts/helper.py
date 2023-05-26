from fontTools import ttLib
tt = ttLib.TTFont('OpenSans-Regular.ttf')
f = open("ex.xml", "w")


loca = tt['loca']

f.write(tt['loca'])
f.close()