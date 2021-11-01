require "define"

CommonVAC={}
CommonVAC.currentCourse = "common"
CommonVAC.count=0;
CommonVAC.corArray={};
CommonVAC.callbackArray={};


function CommonVAC.CheckoutRes(courseid)
    if(type(courseid)=="string")then 
        CommonVAC.currentCourse=courseid; 
        logWarn("courseid设置成功 当前课程为:"..CommonVAC.currentCourse);
    else
        logWarn("courseid设置失败 只能接受string类型");
    end
end


--[[
    非阻塞 
    {{type,id},角色obj,角色动画名字,角色结束动画名字,持续时间}
    callback 阻塞
]]
function CommonVAC.Fast(tbl)
    if(type(tbl)~="table")then
        return;
    end
    CommonVAC.count = CommonVAC.count + 1;
    CommonVAC.corArray[tostring(CommonVAC.count)] = coroutine.start(CommonVAC.OnFast,tostring(CommonVAC.count),tbl);    
    return CommonVAC;
end

function CommonVAC.Callback(func)
    if(type(func)~="function")then
        return;
    end
    -- log("当前增加回调函数的count为:"..CommonVAC.count);
    local currentCount = tostring(CommonVAC.count);
    if(CommonVAC.callbackArray[currentCount]==nil)then
        CommonVAC.callbackArray[currentCount]={};
    end
    table.insert(CommonVAC.callbackArray[currentCount],func);
    return CommonVAC;
end


function CommonVAC.OnFast(key,tbl)
    local duration = 0;    
    -- 播放声音 --获取当前时长
    duration = CommonVAC.PlayMp3(tbl);
    -- 播放开始动画
    local spineTime = CommonVAC.PlaySpine(tbl[2],tbl[3]);
    -- 持续时间 --如果有设置持续时长,更新为持续时长
    if(type(tbl[5])=="number")then
        duration = tbl[5];
    else
        duration = duration>spineTime and duration or spineTime;
    end
    coroutine.wait(duration);
    -- 播放结束动画
    CommonVAC.PlaySpine(tbl[2],tbl[4]);
    -- 停止声音
    CommonVAC.StopSound(tbl)
    -- 播放回调
    if(CommonVAC.callbackArray[key]==nil)then return;end
    for i=1,#CommonVAC.callbackArray[key] do
        local func = table.remove(CommonVAC.callbackArray[key],1);
        func();
    end    
end

function CommonVAC.PlayMp3(tbl)
    if(tbl[1]~=nil and type(tbl[1])=="table")then
        local soundInfo = tbl[1];
        if(soundInfo[1]~=nil and type(soundInfo[1])=="string")then
            local _type = soundInfo[1];
            if(soundInfo[2]~=nil and type(soundInfo[2])=="number")then
                local id = soundInfo[2];
                return soundMgr:PlayClip(id,CommonVAC.currentCourse,_type);
            else
                return 0;
            end
        else
            return 0;
        end
    else 
        return 0;
    end
end
function CommonVAC.StopSound(tbl)   
    if(tbl[1]~=nil and type(tbl[1])=="table")then 
        local soundInfo = tbl[1];
        if(soundInfo[1]~=nil and type(soundInfo[1])=="string")then
            if(soundInfo[1]=="bgm" and tbl[5]==nil)then                
            else
                soundMgr:Stop(soundInfo[1]);
            end
        end
    end
end
function CommonVAC.PlaySpine(obj,anim)
    if(obj~=nil)then
        if(type(anim)=="string")then
            return spineMgr:DoAnimation(obj,anim);
        end
    else
        return 0;
    end
end

function CommonVAC.Disable()
    for k,v in pairs(CommonVAC.corArray) do
        if(v~=nil)then
            coroutine.stop(v);
        end
    end
    CommonVAC.corArray={};
    CommonVAC.count=0;
end

return CommonVAC;
