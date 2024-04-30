x = 0

function setup()              
    print('setup second')
end

function run()
    sleep(200)
    x = x + 1
    print('run second ' .. x)
end

function cleanup()
    print('done')
end