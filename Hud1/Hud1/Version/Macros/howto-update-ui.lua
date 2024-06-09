Label = "How-to update this Macro UI"
Description = "Coding sample on how to do update / log to the Macro UI."

function Setup()
	Print("Setup")
	Sleep(200)
	Print("Setup ■  ")
	Sleep(100)
	Print("Setup ■■ ")
	Sleep(100)
	Print("Setup ■■■")
	Sleep(200)
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
	Sleep(200)
	Print("Cleanup ■■")
	Sleep(100)
	Print("Cleanup ■")
	Sleep(100)
	Print("Cleanup")
	Sleep(200)
	Print()
end