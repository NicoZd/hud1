label = "Repeat left mouse click"
description = "Press and release the left mouse button every 100ms."

function setup()  
end

function run()
	sleep(100)
	print("Down")
	sleep(100)
	print("Up")
end

function cleanup()
	print("")
end