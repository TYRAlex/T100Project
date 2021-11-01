require "define"

CoroutineArray={}
CoroutineArray.array={};

function CoroutineArray.Add(key,func,...)
    if(type(key)~="string")then logWarn("key 的属性限制为 string"); return;end
    if(type(func)~="function")then logWarn("func 的属性限制为 function"); return;end
    if(CoroutineArray.array==nil or type(CoroutineArray.array)~="table")then
        -- log("新建CoroutineArray.array")
        CoroutineArray.array={};
    else
        --检查是否存在重复的
        for k,v in pairs(CoroutineArray.array) do
            if(k==key)then
                --释放之前的        
                coroutine.stop(CoroutineArray.array[key]);
                break;
            end
        end
    end
    --增加新的 
    CoroutineArray.array[key]=coroutine.start(func,...);
    return CoroutineArray;
end

function CoroutineArray.Remove(key)
    if(CoroutineArray.array[key]~=nil)then
        coroutine.stop(CoroutineArray.array[key]);
        -- log("检查到对应的协程,已经关闭");
    end
    return CoroutineArray;
end

function CoroutineArray.Clear()
    for k,v in pairs(CoroutineArray.array) do
        if(v~=nil)then 
            coroutine.stop(v);
            -- log("关闭协程:"..k);
        end
    end
    CoroutineArray.array={};
end

return CoroutineArray;