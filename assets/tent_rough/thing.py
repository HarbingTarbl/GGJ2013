from PIL import Image
from PIL import ImagePath

name = raw_input()
img = Image.open(name)
x, y = img.size
data = img.getdata()
t = (255, 255, 255)
points = []


i = y - 1
j = x - 1

while i >= 0:
    j = 0
    while j < x:
        if(data[j + i*x] != (255, 255, 255, 0)):
            points.append((j, i))
            break
        j += 1
    i -= 1

i = 0
while i < y:
    j = x - 1
    while j >= 0:
        if(data[j + i*x] != (255, 255, 255, 0)):
            points.append((j, i))
            break
        j -= 1
    i += 1


avgx = 0
avgy = 0
for p in points:
    avgx += p[0]
    avgy += p[1]

avgx /= len(points)
avgy /= len(points)

newp = points;

path = ImagePath.Path(newp)
path.compact(10)

f = file(name + ".tex", "w")
for p in path:
    f.write("new Vector2({0}f, {1}f),".format(p[0], p[1]))

f.close()

