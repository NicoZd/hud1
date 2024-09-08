Label = "Scum"
Description = "Repeat pressing and releasing WASD keys to move in a diamond shape. The macro stops if the right mouse button is pressed."

leftRepeatAction = false
diamondAction = false

function OnKeyDown(keyEvent)
	if (keyEvent.key == Key.F5) then
        diamondAction = not diamondAction
		Print("diamondAction" .. (diamondAction and "True" or "False"))
    end
end

function OnMouseDown(button, point)
	Print("OnMouseDown" .. button)
	if button == 5 then
		leftRepeatAction = true
	end
end

function OnMouseUp(button, point)
	Print("OnMouseUp" .. button)
	if button == 5 then
		leftRepeatAction = false
	end
end

function Run()
	if leftRepeatAction then
		Print("Down")
		MouseDown(MouseButton.Left)
		Wait(100)

		Print("Up")
		MouseUp(MouseButton.Left)
		Wait(100)
	end

	if diamondAction then
		waitTime = 1000
		Print("Left Back")
		KeyDown(Key.A)
		KeyDown(Key.S)
		Wait(waitTime)
		KeyUp(Key.A)
		KeyUp(Key.S)

		Print("Left Front")
		KeyDown(Key.A)
		KeyDown(Key.W)
		Wait(waitTime)
		KeyUp(Key.A)
		KeyUp(Key.W)

		Print("Right Front")
		KeyDown(Key.W)
		KeyDown(Key.D)
		Wait(waitTime)
		KeyUp(Key.W)
		KeyUp(Key.D)

		Print("Right Back")
		KeyDown(Key.D)
		KeyDown(Key.S)
		Wait(waitTime)
		KeyUp(Key.D)
		KeyUp(Key.S)
	end
end