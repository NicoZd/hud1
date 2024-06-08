Label = "Sample Realtime Updates"
Description = "Coding sample on how to do realtime updates and move the mouse."

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

	rad = now * 0.003
	MouseMove(math.cos(rad) * 200 + 1920 / 2, math.sin(rad) * 200 + 1080 / 2)
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