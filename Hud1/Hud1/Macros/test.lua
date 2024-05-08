Label = "Sample Realtime Updates"
Description = "Coding sample on how to do realtime updates."

function Setup()
	startMs = Millis()
end

function Run()	
	now = Millis()
	dt = now - startMs
	startMs = now;
	Print(""
		.. "Time and dt: " .. now .. "ms " .. dt .. "ms\n"
		.. "UI Debounce: 100ms \n"
		.. "Version    : " .. _VERSION
		)
end

function OnMouseDown()  
end

function Cleanup()
	Print("")
end