function init()
    return {
        width = 640,
        height = 480
    };
end

function setup()
    window = getWindow(); -- C++側で定義されてる.

    -- 始点と終点を定義して間に線を引く.
    a = createObject("Line",100,200,300,50); -- 始点を用意
    b = createObject("Line",80,30,250,250); -- 終点

    -- C++側に渡す.
    window:setObject(a);
    window:setObject(b);
end