Label = "How-to control the Mouse Position"
Description = "Coding sample on how to move the Mouse on the Primary Screen."

function Setup()
	Print("Moving the mouse in circles")
	startMs = Millis()
end

function Run()	
	now = Millis()
	dt = now - startMs	
	rad = now * 0.003
	MouseMove(math.cos(rad) * 200 + 1920 / 2, math.sin(rad) * 200 + 1080 / 2)
end

function Cleanup()
	Print()
end