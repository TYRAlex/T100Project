--require("functions")

CtrlNames = {
	rightui = "RightUI",
	game = "Game",
	login = "Login",
	toolbar = "Toolbar",
	updateWarn = "UpdateWarn",
	selectSeries = "SelectSeries",
	selectClass = "SelectClass",
	selectAge = "SelectAge",
	selectCourse = "SelectCourse",
	selectHistory = "SelectHistory",
	entrance = "Entrance",
	teacher = "Teacher",
	teachCourse = "TeachCourse",
	download = "Download",
	sdownload = "SDownload",
	search = "Search",
	history = "History",
	detail = "Detail",
	agreement = "Agreement",
	dialog = "Dialog",
	listloading = "ListLoading",
	studentDialog = "StudentDialog",
	student = "Student",
	laud ="Laud",
	pictureUpload = "PictureUpload",
	CourseSchedule="CourseSchedule"
}

PanelNames = {
	"RightUIPanel",
	"GamePanel",
	"LoginPanel",
	"UpdateWarnPanel",
	"SelectSeriesPanel",
	"SelectClassPanel",
	"SelectAgePanel",
	"SelectCoursePanel",
	"DownloadPanel",
	"SDownloadPanel",
	"SearchPanel",
	"HistoryPanel",
	"DetailPanel",
	"AgreementPanel",
	"DialogPanel",
	"ToolbarPanel",
	"SelectHistoryPanel",
	"ListLoadingPanel",
	"EntrancePanel",
	"TeacherPanel",
	"TeachCoursePanel",
	"StudentPanel",
	"StudentDialogPanel",
	"LaudPanel",
	"PictureUploadPanel",
	"CourseSchedulePanel"
}

Util = LuaFramework.Util;
log = Util.Log
logWarn = Util.LogWarning
logError = Util.LogError

GameObject = UnityEngine.GameObject
GamePanel = { }
GamePanel.background = GameObject.Find("MainCanvas").transform

function newObject(prefab)
	return GameObject.Instantiate(prefab);
end

function destroy(obj)
	GameObject.Destroy(obj);
end
-- AppConst = LuaFramework.AppConst;
-- LuaHelper = LuaFramework.LuaHelper;
-- ByteBuffer = LuaFramework.ByteBuffer;
Droper = LuaFramework.Droper;
Drager = LuaFramework.Drager;
-- Application = UnityEngine.Application;--导入Application

-- resMgr = LuaHelper.GetResManager();
-- panelMgr = LuaHelper.GetPanelManager();
soundMgr = ILFramework.SoundManager.instance
-- networkMgr = LuaHelper.GetNetManager();

courseMgr = ILFramework.ResourceManager.instance
spineMgr = ILFramework.SpineManager.instance
connect3DofMgr=ILFramework.ConnectAndroid.Instance
functionOf3DofMgr=ILFramework.FunctionOf3Dof
-- sqliteMgr = LuaHelper.GetSqliteManager();
-- downloadMgr = LuaHelper.GetDownloadManager();
-- gameMgr = LuaHelper.GetGameManager();
-- ApkUpadteMgr = LuaHelper.GetApkUpdateManager();
-- objectPoolMgr = LuaHelper.GetObjectPoolManager();
-- uploadMgr = LuaHelper.GetUploadManager();
xdeviceMgr = ILFramework.XDeviceManager.instance
logicMgr = ILFramework.LogicManager.instance
videoMgr = logicMgr
-- WWW = UnityEngine.WWW;
-- WWWForm = UnityEngine.WWWForm;
--GameObject = UnityEngine.GameObject;
Component = UnityEngine.Component;
RectTransform = UnityEngine.RectTransform;
BoxCollider2D = UnityEngine.BoxCollider2D;
BoxCollider = UnityEngine.BoxCollider;
PolygonCollider2D = UnityEngine.PolygonCollider2D;
MeshCollider = UnityEngine.MeshCollider;
Sprite = UnityEngine.Sprite;
Rect = UnityEngine.Rect;
Input = UnityEngine.Input;
-- offlineTest = false;--离线模式
-- useFakeCourseData = false;--使用假的课程数据
-- clickDelay = 0.4;--点击延时
