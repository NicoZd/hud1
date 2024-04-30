x = 0

function setup()              
    print('setup first')
    sleep(500)
end

function run()
    sleep(500)
    x = x + 1
    print('run first ' .. x)
end

function cleanup()
    print('cleanup first')
    sleep(500)
    print('done')
end