Label = "How-to update this Macro UI"
Description = "Coding sample on how to do update / log to the Macro UI."

function Setup()	
	Print("Setup")
	Wait(200)
	Print("Setup ■  ")
	Wait(100)
	Print("Setup ■■ ")
	Wait(100)
	Print("Setup ■■■")
	Wait(200)
	startMs = Millis()
end

function Run()	
	now = Millis()
	dt = now - startMs
	startMs = now;
	Print(""
		.. "Time and dt: " .. now .. "ms " .. dt .. "ms\n"
		.. "UI Debounce: 50ms \n"
		.. "Version    : " .. _VERSION
		)
end

function Cleanup()
	Print("Cleanup ■■■")
	Wait(200)
	Print("Cleanup ■■")
	Wait(100)
	Print("Cleanup ■")
	Wait(100)
	Print("Cleanup")
	Wait(200)
	Print()
end