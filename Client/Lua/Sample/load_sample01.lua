local M = {};
local modname=...;
function M.Call()
    print(string.format("[load_sample01]chunk:%s",modname))
end
return M;