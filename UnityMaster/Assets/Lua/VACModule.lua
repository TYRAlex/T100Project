require "define"

--[[
    VACModule: Voice action coordination Module
    声音动作协作模块

    VACModule
        参数:
            courseId(string):当前课程的id
            role(gameobject):当前角色实体(animation角色)
            defaultSpeak(string):当前角色说话默认动作
            idle(string):当前角色待机默认动作
            voiceId(number):当前角色语音对应id

        {key,obj,soundId,muteTbl,{SEId,delay,duration,startAnim,endAnim},preFunc,callback}
]]

VACModule={role=nil,courseId=nil,currentSource=nil,type="voice",isRole=false,FastMap=nil,speakPool=nil};
-- 构造函数
function VACModule:new(o,role,courseId)
    o = o or {}
    o.courseId=courseId;
    o.role=role;
    o.courseId=courseId;
    setmetatable(o,self);
    self.__index=self;
    return o;
end

-- public function
function VACModule:SetConfig(speak,idle,speakPool)
    self:setSpeak(speak);
    self:setIdle(idle);
    self:setSpeakPool(speakPool);
    self.isRole = true;
    return self;
end
function VACModule:Hide()
    self.role:SetActive(false);
    return self;
end
function VACModule:Show()
    self.role:SetActive(true);
    return self;
end
function VACModule:FastSpeak(key)
    -- log("说话角色是:"..self.role.name);
    local _type = type(key);
    if(_type=="string")then
        --通过key调用    
        self.fastspk = coroutine.start(self.OnFastSpeakByKey,self,key);
    elseif(_type=="number")then
        --直接播放语音
        self.fastspk = coroutine.start(self.OnFastSpeakById,self,key);
    end
end
function VACModule:InsertFastEventList(tbl)
    if(self.FastMap==nil)then self.FastMap={};end
    if(type(tbl)~="table")then return self;end
    for i=1,#tbl do
        self:InsertFastEvent(tbl[i]);
    end
    return self;
end
function VACModule:CheckoutCommon()
    -- log("语音资源切换到通用语音,语音播放完后恢复当前语音资源");    
    self.currentSource="common";
    return self;
end
function VACModule:Disable()
    if(self.fastspk)then coroutine.stop(self.fastspk);end
    if(self.ospine)then coroutine.stop(self.ospine);end
    if(self.orolectl)then coroutine.stop(self.orolectl);end
    if(self.ospinectl)then coroutine.stop(self.ospinectl);end
    if(self.osectl)then coroutine.stop(self.osectl);end
    if(self.ospine)then coroutine.stop(self.ospine);end
    self:SimpleIdle();
end

--private function
function VACModule:setRole(role)
    if(type(role)~=nil)then
        self.role=role;
    end
    return self;
end
function VACModule:setSpeak(str)
    if(type(str)=="string")then
        self.speak=str;
    else
        logWarn("VACModule:SetSpeak 设置失败");
    end
    return self;
end
function VACModule:setIdle(str)
    if(type(str)=="string")then
        self.idle=str;
    else
        logWarn("VACModule:SetIdle 设置失败");
    end
    return self;
end
function VACModule:setSpeakPool(tbl)
    if(type(tbl)~="table")then 
        logWarn("VACModule:SetSpeakPool 只接受 table 型数据");
        return self;
    end
    self.speakPool={};
    for i=1,#tbl do
        if(type(tbl[i])~="string")then
            logWarn("VACModule:SetSpeakPool table 中数据要求是string型")
            return self;
        else
            table.insert(self.speakPool,tbl[i]);
        end
    end
    self.poolFlag=true;
    log("VACModule:SetSpeakPool 设置成功");
    return self;
end
-- {key,obj,soundId,muteTbl,{SEId,delay,duration,startAnim,endAnim},preFunc,callback}
function VACModule:InsertFastEvent(tbl)
    if(type(tbl[1])~="string")then return false; end
    -- tbl[2]: obj 为unityobj类型无法确认
    if(type(tbl[3])~="number" or tbl[3]<0)then tbl[3]=nil;end
    --检测muteTbl
    if(self:CheckMuteTbl(tbl[4])==false)then tbl[4]=nil;end
    --检测spinTbl
    self:CheckSpineTbl(tbl[5]);
    if(type(tbl[6])~="function")then tbl[6]=nil;end
    if(type(tbl[7])~="function")then 
        -- log("callback is not function")
            tbl[7]=nil;
    else
        -- log("callback is function")
    end
    -- log("FastEvent成功更新")
    table.insert(self.FastMap,tbl);
end

function VACModule:GetFastEvent(key)
    log("传入的key是:"..key);
    if(type(key)~="string")then
        logWarn("VACModule:GetFastEvent 接受数据类型为 string");
        return nil;
    end
    -- log("这个角色是:"..data.role.name);
    for i=1,#(self.FastMap) do
        if(self.FastMap[i][1]==key)then
            log("找到对应的方法")
            return self.FastMap[i];
        end
    end
    log("没有找到对应的方法")
    return nil;
end
function VACModule.OnFastSpeakById(self,id)
    -- log("传入的id:"..id.." 语音资源类型:"..tmp.type);
    self.ospine = coroutine.start(self.OnCommomSpine,self);
    if(self.currentSource==nil)then self.currentSource=self.courseId;end
    local duration = soundMgr:PlayClip(id,self.currentSource,self.type);
    -- log("这个duration:"..duration);
    coroutine.wait(duration);
    if(self.ospine)then coroutine.stop(self.ospine); end 
    self:SimpleIdle();
    if(self.fastspk)then coroutine.stop(self.fastspk); end
    self.currentSource=nil;
end
--   1   2     3       4      5-1  5-2   5-3        5-4      5-5       6       7
-- {key,obj,soundId,muteTbl,{SEId,delay,duration,startAnim,endAnim},preFunc,callback}
function VACModule.OnFastSpeakByKey(self,key)
    local tbl = self:GetFastEvent(key);
    if(tbl==nil)then return; end
    -- preFuc
    if(tbl[6]~=nil)then tbl[6]();end
    -- 语音控制
    local roleTbl={};
    roleTbl.id=tbl[3];
    roleTbl.muteTbl=tbl[4];
    roleTbl.callback=tbl[7];

    self.orolectl = coroutine.start(self.OnRoleCtl,self,roleTbl);

    local spineTbl={};
    local seTbl={};
    spineTbl.obj=tbl[2];
    if(tbl[5]==nil)then
        seTbl.id=nil;
        seTbl.delay=nil;
        seTbl.duration=nil;
        spineTbl.startAnim=nil;
        spineTbl.endAnim=nil;
    else
        seTbl.id=tbl[5][1];
        seTbl.delay=tbl[5][2];
        seTbl.duration=tbl[5][3];
        spineTbl.startAnim=tbl[5][4];
        spineTbl.endAnim=tbl[5][5];
        -- spine控制
        self.ospinectl = coroutine.start(self.OnSpineCtl,spineTbl);
        -- 音效控制 id,delay,duration        
        self.osectl = coroutine.start(self.OnSECtl,self,seTbl);
    end
end

function VACModule.OnRoleCtl(self,roleInfo)
    -- log("这个是主角,播放语音id:"..roleInfo.id)
    if(roleInfo.id~=nil)then             
        if(self.currentSource==nil)then self.currentSource=self.courseId;end
        log("播放声音的信息,id:"..roleInfo.id.." currentSource:"..self.currentSource.." type:"..self.type);
        local duration = soundMgr:PlayClip(roleInfo.id,self.currentSource,self.type);
        if(roleInfo.muteTbl~=nil)then
            -- log("启动muteSpine功能")
            self.ospine = coroutine.start(self.OnMuteSpine,self,roleInfo.muteTbl);
        else
            -- log("这个muteSpine是空的");
            self.ospine = coroutine.start(self.OnCommomSpine,self);
        end
        -- log("这个延时的时间长度为:"..duration)
        coroutine.wait(duration);
        -- log("延时等待结束")
        self:SimpleIdle();
    end
    if(self.ospine)then coroutine.stop(self.ospine); end  
    self.currentSource=nil;
    if(roleInfo.callback~=nil)then roleInfo.callback();end
end


-- function VACModule.OnSpineCtl(tmp,anim1,anim2)
function VACModule.OnSpineCtl(spineInfo)
    if(spineInfo.obj==nil)then return; end
    if(spineInfo.startAnim==nil)then return; end
    local timeline = spineMgr:DoAnimation(spineInfo.obj,spineInfo.startAnim,false);
    coroutine.wait(timeline);
    if(spineInfo.endAnim==nil)then anim2=anim1;end
    spineMgr:DoAnimation(spineInfo.obj,spineInfo.endAnim);
end
-- function VACModule.OnSECtl(tmp,id,delay,duration)
function VACModule.OnSECtl(self,SEInfo)
    if(SEInfo.delay~=nil)then
        coroutine.wait(SEInfo.delay);
    end
    if(SEInfo.id~=nil)then
        log("播放音效") 
        if(self.currentSource==nil)then self.currentSource=self.courseId;end
        if(SEInfo.duration==nil or SEInfo.duration==0)then
            log("duration为零 获取声音长度")
            SEInfo.duration = soundMgr:PlayClip(SEInfo.id,self.currentSource,"sound");
        else
            log("已经有duration 无须再次获取长度")
            soundMgr:PlayClip(SEInfo.id,self.currentSource,"sound");
        end
        self.currentSource=nil;
    end
    coroutine.wait(SEInfo.duration);
    soundMgr:Stop("sound");
end


-- function VACModule:RandomSpeak(self)
function VACModule:RandomSpeak()
    -- log("poolFlag的数据类型为:"..type(tmp.poolFlag))
    if(self.poolFlag==true)then
        -- log("poolFlag:true")
        math.randomseed(tostring(os.time()):reverse():sub(1, 7));
        return self.speakPool[math.random(1,#(self.speakPool))];
    else
        -- log("poolFlag:false")
        return self.speak;
    end
end
function VACModule:CheckMuteTbl(muteTbl)
    -- 检查是否为table
    if(type(muteTbl)~="table")then 
        logWarn("传入的muteTbl必须为table型");
        return false; 
    end
    -- 检查table中长度是否都为2
    for i=1,#muteTbl do
        if(#(muteTbl[i])~=2)then
            logWarn("muteTbl中数组长度必须控制为2");
            return false;
        end
    end
    return true;
end
function VACModule:CheckSpineTbl(spineTbl)
    if(type(spineTbl)~="table")then return;end
    if(type(spineTbl[1])~="number" or spineTbl[1]<0)then spineTbl[1]=nil;end
    if(type(spineTbl[2])~="number" or spineTbl[2]<0)then spineTbl[2]=nil;end
    if(type(spineTbl[3])~="number" or spineTbl[3]<0)then spineTbl[3]=nil;end
    if(type(spineTbl[4])~="string")then spineTbl[4]=nil;end
    if(type(spineTbl[5])~="string")then spineTbl[5]=nil;end
end
function VACModule:UpdateDuration(num)
    if(data.duration~=nil)then
        return;
    end
    data.duration = num;    
end

--没有mute的时候语音动作切换协程
function VACModule.OnCommomSpine(self)
    while(true)
    do
        self:SimpleSpeak();
    end
end
--有mute的时候语音动作切换协程
function VACModule.OnMuteSpine(self,muteTbl)
    self:SimpleSpeak();
    --开始解析mute数组
    -- log("开始解析mute数组");
    local endTime = 0;
    for i=1,#muteTbl do
        coroutine.wait(muteTbl[i][1]-endTime);
        self:SimpleIdle();
        coroutine.wait(muteTbl[i][2]-muteTbl[i][1]);
        self:SimpleSpeak();
        endTime = muteTbl[i][2]
    end
    self.OnCommomSpine(self);
    -- log("结束解析mute数组");
end

function VACModule:SimpleSpeak()
    local aniName = self:RandomSpeak();
    -- log("角色是:"..self.role.name.." 动画名字:"..aniName);
    tmpTime = spineMgr:DoAnimation(self.role,aniName);
    -- log("这个语音动画时间长度为:"..tmpTime);
    coroutine.wait(tmpTime);
end

function VACModule:SimpleIdle()
    spineMgr:DoAnimation(self.role,self.idle);
    -- log("播放待机动画")
end

return VACModule;