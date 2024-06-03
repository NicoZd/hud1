Label = "Instruction limitations"
Description = "Check instruction limitations"

c = 0
while c < 2000 do
	c = c + 1
	Print("" .. c);
end

function Setup()
	c = 0
	while c < 2000 do
		c = c + 1
		Print("" .. c);
	end
end

function Run()	
	c = 0
	while c < 2000 do
		c = c + 1
		Print("" .. c);
	end
end

function Cleanup()
	c = 0
	while c < 2000 do
		c = c + 1
		Print("" .. c);
	end
end