label = "Press left mouse"
description = "Press the left mouse button once."

function setup()    
end

function run()
	print("Left mouse button is pressed until manually released.")
	sleep(2000)
	running = false
end

function cleanup()
	print("")
end