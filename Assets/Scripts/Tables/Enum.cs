
public enum Goods_Type
{
	None = 1, // 재화 종류 Null 값
	AccountExp = 2, // 계정 경험치
	Ruby = 3, // 루비
	Heart = 4, // 하트
	StarPoint = 5, // 별 포인트(상자 열기)
	RankingPoint = 6, // 랭킹 포인트
	FriendPoint = 7, // 우정 포인트
	SkillUpKeeper = 8, // 성급권(수호자)
	SkillUpHitter = 9, // 성급권(타격대)
	SkillUpRanger = 10, // 성급권(공격수)
	SkillUpWizard = 11, // 성급권(도사)
	SkillUpHealer = 12, // 성급권(치유사)
	EquipUpWeapon = 13, // 장비권(무기)
	EquipUpArmor = 14, // 장비권(방어구)
	EquipUpAccessory = 15, // 장비권(장신구)
	Gold = 16, // 골드
	GuildExp = 17, // 길드 경험치
	GuildPoint = 18, // 길드 포인트
	TrainingPoint = 19, // 수련 포인트
	RevengePoint = 20, // 복수 포인트
	SmilePoint = 21, // 스마일 포인트
	TreasureDetectMap_Terrapin = 22, // 자라섬 보물지도
	TreasureDetectMap_Coconut = 23, // 야자섬 보물지도
	TreasureDetectMap_Ice = 24, // 얼음섬 보물지도
	TreasureDetectMap_Lake = 25, // 호수섬 보물지도
	TreasureDetectMap_Black = 26, // 검은섬 보물지도
	SweepTicket = 27, // 소탕권
	Card = 28, // 카드 (캐릭터)
	Box = 29, // 상자 (카드 상자)
}

public enum Grade_Type
{
	None = 1, // 카드 등급 Null 값
	Grade_C = 2, // C등급
	Grade_B = 3, // B등급
	Grade_A = 4, // A등급
	Grade_S = 5, // S등급
	Grade_SS = 6, // SS등급 (전설등급)
}

public enum Price_BuyType
{
	None = 1, // 결제 수단 Null 값
	PayFree = 2, // 무료 결제
	PayCash = 3, // 현금 결제
	PayGold = 4, // 골드 결제
	PayFriendPoint = 5, // 우정 포인트 결제
	PayHonorPoint = 6, // 명예 포인트 결제
	PayRuby = 7, // 루비 결제
}

public enum BuyConditionType
{
	None = 1, // None
	TimeReset = 2, // TimeReset
	Once = 3, // Once
}

public enum Real_BuyType
{
	None = 1, // None
	FixedFare = 2, // FixedFare
}

public enum ClassType
{
	None = 1, // 클래스 종류 Null 값
	ClassType_Keeper = 2, // ClassType_Keeper
	ClassType_Hitter = 3, // ClassType_Hitter
	ClassType_Ranger = 4, // ClassType_Ranger
	ClassType_Wizard = 5, // ClassType_Wizard
	ClassType_Healer = 6, // ClassType_Healer
}

public enum AIType
{
	None = 1, // AIType Null 값
	AIType_Keeper = 2, // AIType_Keeper
	AIType_Hitter = 3, // AIType_Hitter
	AIType_Ranger = 4, // AIType_Ranger
	AIType_Wizard = 5, // AIType_Wizard
	AIType_Healer = 6, // AIType_Healer
}

public enum SpawnType
{
	None = 1, // SpawnType Null 값
	Normal = 2, // Normal
	Wave_StartInterval = 3, // Wave_StartInterval
	Wave_EndInterval = 4, // Wave_EndInterval
	Wave_Move = 5, // Wave_Move
}

public enum Overlap_Check
{
	None = 1, // Overlap_Check Null 값
	TRUE = 2, // TRUE
	FALSE = 3, // FALSE
}

public enum SkillType
{
	Leader = 1, // Leader
	Active = 2, // Active
	Max = 3, // Max
}

public enum TEXT_UI
{
	NONE = 1, // TEXT_UI Null 값
	GUIDE = 2, // GUIDE
	OK = 3, // 확인
	CANCEL = 4, // 취소
	GOLD = 5, // 골드
	CASH = 6, // 루비
	HEART = 7, // 하트
	BOX = 8, // 상자
	EXIT = 9, // 나가기
	LEVEL = 10, // 레벨
	EXP = 11, // EXP
	LV = 12, // LV.
	BATTLE_POWER = 13, // 전투력
	HP = 14, // 체력
	AP = 15, // 공격
	DP = 16, // 방어
	YES = 17, // 예
	NO = 18, // 아니오
	QUIT = 19, // 종료
	NOTICE_WARNING = 20, // 알림
	MATCH = 21, // 매칭
	MATCH_TRY = 22, // MATCH_TRY
	NETWORK_ERROR = 23, // 통신 오류
	RETRY = 24, // 재시도
	GIVEUP = 25, // GIVEUP
	GIVEUP_DESCRIPTION = 26, // 보상을 포기하게 됩니다. 중단하시겠습니까?
	BOX_FULL = 27, // 상자가 가득 차 승리 시 상자를 획득할 수 없습니다. 계속 진행하시겠습니까?
	INFO = 28, // 정보
	POSITION = 29, // 위치
	LEADER = 30, // 리더
	USE = 31, // 사용
	FEATURE = 32, // 특징
	MAIN = 33, // 메인
	SKILL_UP = 34, // 스킬업
	LEVEL_UP = 35, // 레벨업
	EQUIP_UP = 36, // 장비업
	CARD_DECK_USE = 37, // 사용 덱
	CARD_DECK_OWN = 38, // 보유 카드
	CARD_DECK_NOTHING = 39, // 미보유 카드
	CARD_DECK_AREA = 40, // 지역
	CARD_DECK_SLOT1 = 41, // (사용 안함) 1
	CARD_DECK_SLOT2 = 42, // (사용 안함) 2
	CARD_DECK_SLOT3 = 43, // (사용 안함) 3
	CARD_DECK_CHANGE_CHOISE = 44, // 교체할 캐릭터를 선택하세요.
	CARD_DECK_CHANGE_POSITION = 45, // 변경할 위치를 선택하세요.
	CARD_DECK_AREA_GET = 46, // {0}지역에서 획득할 수 있습니다.
	CARD_DECK_TITLE_SKILL = 47, // 성급
	CARD_DECK_TITLE_EQUIP = 48, // 장비
	CARD_DECK_OWN_EMPTY = 49, // 다양한 카드를 더 모아보세요.
	CARD_DECK_NOTHING_EMPTY = 50, // 새로운 카드의 등장을 기대해 주세요!
	LEVEL_MAX = 51, // MAX
	AREA_1 = 52, // 거룡반점              지역1 이름
	AREA_2 = 53, // 비밀의 유령섬      지역2 이름
	AREA_3 = 54, // M 타워 연구실      지역3 이름
	AREA_4 = 55, // 허름한 해적선      지역4 이름
	AREA_5 = 56, // 수노인의 거처     지역5 이름
	AREA_6 = 57, // 불꽃의 왕좌         지역6 이름
	T1_BOX = 58, // 나무    PvP 매칭보상상자티어1
	T2_BOX = 59, // 백은    PvP 매칭보상상자티어2
	T3_BOX = 60, // 황금    PvP 매칭보상상자티어3 (상점에도판매)
	T4_BOX = 61, // 마법    PvP 매칭보상상자티어4 (상점에도판매)
	T5_BOX = 62, // 플래티넘  PvP 매칭보상상자티어5 (상점에도판매)
	F1_BOX = 63, // 번개    무료상자1 <보물 찾기 컨텐츠에적용예정>
	F2_BOX = 64, // 눈꽃    무료상자2 <보물 찾기 컨텐츠에 적용 예정>
	F3_BOX = 65, // 무지개    무료상자3 <보물 찾기 컨텐츠에 적용 예정>
	GOODS_ACCOUNT_EXP = 66, // 계정 경험치
	GOODS_RUBY = 67, // 루비
	GOODS_HEART = 68, // 하트
	GOODS_STAR_POINT = 69, // 별 포인트(상자 열기)
	GOODS_RANKING_POINT = 70, // 랭킹 포인트
	GOODS_FRIEND_POINT = 71, // 우정 포인트
	GOODS_SKILLUP_KEEPER = 72, // 성급권(수호자)
	GOODS_SKILLUP_HITTER = 73, // 성급권(타격대)
	GOODS_SKILLUP_RANGER = 74, // 성급권(공격수)
	GOODS_SKILLUP_WIZARD = 75, // 성급권(도사)
	GOODS_SKILLUP_HEALER = 76, // 성급권(치유사)
	GOODS_EQUIPUP_WEAPON = 77, // 장비권(무기)
	GOODS_EQUIPUP_ARMOR = 78, // 장비권(방어구)
	GOODS_EQUIPUP_ACCESSORY = 79, // 장비권(장신구)
	CARD = 80, // 카드
	SHOP = 81, // 상점
	SLOT = 82, // 상자슬롯
	ADVENTURE = 83, // 모험
	EQUIPITEMNAME_ACC_0 = 84, // 장비 : 장신구 이름 1 단계 (1~10레벨)
	EQUIPITEMNAME_ACC_1 = 85, // 장비 : 장신구 이름 2 단계 (11~20레벨)
	EQUIPITEMNAME_ACC_2 = 86, // 장비 : 장신구 이름 3 단계 (21~30레벨)
	EQUIPITEMNAME_WEAPON_0 = 87, // 장비 : 무기 이름 1 단계 (1~10레벨)
	EQUIPITEMNAME_WEAPON_1 = 88, // 장비 : 무기 이름 2 단계 (11~20레벨)
	EQUIPITEMNAME_WEAPON_2 = 89, // 장비 : 무기 이름 3 단계 (21~30레벨)
	EQUIPITEMNAME_ARMOR_0 = 90, // 장비 : 방어구 이름 1 단계 (1~10레벨)
	EQUIPITEMNAME_ARMOR_1 = 91, // 장비 : 방어구 이름 2 단계 (11~20레벨)
	EQUIPITEMNAME_ARMOR_2 = 92, // 장비 : 방어구 이름 3 단계 (21~30레벨)
	SC_TREASURE = 93, // 보물찾기
	SC_ACHIEVE = 94, // 업적
	SC_SECRET_EXCHANGE = 95, // 비밀 거래
	SC_RANKING = 96, // 랭킹
	SC_STRANGESHOP = 97, // 수상한 상점
	SC_OPTION = 98, // 환경설정
	NOT_ENOUGH_MANA = 99, // 전투 : 스킬 사용 시 마나 부족할 때 -> 쿨타임 삭제로 아직 사용할 수 없습니다로 변경
	NUMBER_AREA = 100, // {0} 지역 : 지역 넘버는 데이터 연동 필요
	GUARANTEE_GRADE = 101, // 확정 등급 :
	OPEN = 102, // 열기 버튼에 사용
	INSTANT_OPEN = 103, // 즉시 열기 버튼
	NOT_ENOUGH_STAR_POINT = 104, // 별 포인트 부족 알림 문자 (점멸 기능)
	NOTHING = 105, // 없음
	GOODS_GOLD = 106, // 골드 이름
	NETWORK_ERROR_DESC = 107, // 통신이 원활하지 않습니다. 네트워크 상태를 확인해보세요. 초기화면으로 돌아갑니다.
	NOTICE_PREPARE = 108, // 준비중인 컨텐츠 입니다.
	LOADING = 109, // 로딩 중입니다....
	GUILD = 110, // 길드
	GUILD_RECOMMEND = 111, // 추천길드
	MEMBERS = 112, // 인원
	GUILD_JOIN = 113, // 가입
	GUILD_JOIN_HOLDON = 114, // 가입대기
	GUILD_SEARCH = 115, // 길드검색
	SEARCH = 116, // 검색
	GUILD_SETUP = 117, // 길드창설
	EMBLEM = 118, // 엠블럼
	EMBLEM_RANDOM = 119, // 무작위
	EMBLEM_PATTERN = 120, // 문양
	EMBLEM_LOGO = 121, // 로고
	GUILD_NAME = 122, // 길드명
	GUILD_JOIN_APPROVAL = 123, // 길드 가입 방식
	ON = 124, // ON
	OFF = 125, // OFF
	GUILD_INTRO = 126, // 소개글
	GUILD_INTRO_INFO = 127, // 최대 30자까지 작성가능
	GUILD_SETUP_INFO = 128, // 엠블럼과 길드명은 창설 후 변경이 불가능합니다.
	GUILD_JOIN_INFO = 129, // 자유, 승인 버튼을 눌러 승인 방식을 전환할 수 있습니다.  길드원 목록에서 설정을 변경할 수 있습니다.
	RANK = 130, // 위
	GUILD_MEMBERS = 131, // 길드원
	SUPPORT_SCORE = 132, // 카드 지원 수
	GUILD_SHOP = 133, // 길드상점
	GUILD_RAID = 134, // 레이드
	DAY = 135, // 일
	HOUR = 136, // 시간
	MINUTE = 137, // 분
	BEFORE = 138, // 전
	CARD_CARRY = 139, // 현재보유
	SUPPORT = 140, // 지원
	GET_CARD = 141, // 카드를 받았습니다.
	REQUEST = 142, // 카드요청
	CHAT = 143, // 채팅
	CARD_PICK = 144, // 캐릭터 선택
	BATTLE_RANK = 145, // 가맹점
	ENTER_NUMBER = 146, // 참여횟수
	WIN_NUMBER = 147, // 승리횟수
	PLACE_INFO = 148, // 현재위치
	GUILD_BREAKUP = 149, // 길드해체
	GUILD_WITHDRAW = 150, // 길드탈퇴
	COUNT = 151, // 횟수
	SHOP_GRADE = 152, // 현재등급
	DONATION = 153, // 기부
	GUILD_SHOP_INFO_01 = 154, // 1. 길드의 레벨이 높을수록 구매 횟수 및 종류가 다양해집니다.
	GUILD_SHOP_INFO_02 = 155, // 2. 카드교환 및 기부를 통하여 길드를 성장시킬 수 있습니다.
	DONATION_LIST = 156, // 기부자 순위
	DONATION_INFO = 157, // 기부를 통하여 상점을 성장시킬 수 있습니다.
	DETAILS_INFO = 158, // 세부정보
	GUILD_POINT = 159, // 길드 포인트
	INITIAL_POINT = 160, // P
	GUILD_DONA_EXP = 161, // 기부 경험치
	APP_OFF = 162, // 게임을 종료하시겠습니까?
	GUILD_NONE = 163, // 길드없음
	GUILD_LIST_NONE = 164, // 대기 목록이 없습니다.
	GUILD_RECOMMEND_NONE = 165, // 추천 길드가 없습니다.
	TEXT_ENTER = 166, // 입력해주세요.
	CHARACTER_LIST_NONE = 167, // 보유한 캐릭터가 없습니다.
	CHAT_INFO = 168, // {0}님이 카드를 요청합니다.
	APPROVAL = 169, // 승인
	FREE = 170, // 자유
	ACCESS_TIME = 171, // 최근 접속 시간
	REQUEST_INFO = 172, // 요청 목록이 없습니다. 카드를 요청해주세요.
	SEARCH_ENTER = 173, // 길드를 검색하세요.
	GOODS_GUILD_EXP = 174, // 길드 경험치
	GOODS_GUILD_POINT = 175, // 길드 포인트
	QUANTITY_1 = 176, // 개수 :
	GUILD_DONATION = 177, // 길드기부
	JUST = 178, // 방금
	JOIN_WAY_CHANGE = 179, // 가입방식변경
	JOIN_WAY_CHANGE_INFO = 180, // 자유형식으로 변경하면 가입신청명단이 삭제됩니다. 변경하시겠습니까?
	JOIN_REFUSAL = 181, // 가입거절
	JOIN_REFUSAL_INFO = 182, // {0}님의 가입을 거절하시겠습니까?
	GUILD_BREAKUP_INFO = 183, // 길드를 정말 해체하시겠습니까?
	FORCIBLY = 184, // 강제탈퇴
	FORCIBLY_INFO = 185, // {0}님을 탈퇴시키겠습니까?
	GUILD_WITHDRAW_INFO = 186, // 길드를 탈퇴하시겠습니까? 길드기부 및 카드지원수가 초기화됩니다.
	CHAT_JOIN = 187, // {0}님이 가입하셨습니다.
	CHAT_FORCIBLY = 188, // {0}님이 추방당하셨습니다.
	CHAT_WITHDRAW = 189, // {0}님이 탈퇴하셨습니다.
	CHAT_LEVELUP = 190, // 길드 레벨 {0} 달성!
	CHAT_FIOODER = 191, // 채팅 도배가 걸렸습니다. {0}분 후에 시도해주세요.
	CHAT_FIOODER_INFO = 192, // {0}분 후에 도배가 풀립니다.
	GOODS_TRAINING_POINT = 193, // 수련 포인트
	BUY = 194, // 구매
	BUY_INFO = 195, // 아이템을 구매하시겠습니까?
	ENEMY_INFO = 196, // 적 정보
	REWARD_INFO = 197, // 보상 정보
	SWEEP = 198, // 소탕
	BATTLE = 199, // 전투
	BATTLE_FAST = 200, // 빠른 전투
	SWEEP_REMAIN = 201, // 소탕권을 이용하여 빠른 전투를 진행하시겠습니까? 오늘 남은 소탕 횟수 : {0}
	RESULT_GET = 202, // 획득 보상
	ENHANCE_TITLE = 203, // 더 강해지고 싶으세요?
	ENHANCE_INFO_1 = 204, // 캐릭터 카드를 모아 레벨 업을 한다면 더욱 강해질 것입니다!
	REQUEST_TIME = 205, // 요청가능한 시간이 아닙니다.
	CHAT_GUILD_CREATE = 206, // 길드가 창설되었습니다.
	APPROVAL_JOIN_CHANGE = 207, // 승인 가입으로 전환 되었습니다.
	FREE_JOIN_CHANGE = 208, // 자유 가입으로 전환 되었습니다. 가입신청목록이 삭제됩니다.
	CANT_OPEN_YET = 209, // 아직 열 수 없습니다.
	NOT_ENOUGH_ACCOUNT_LEVEL = 210, // 계정레벨이 부족합니다.
	TREASURE_BOAT_1 = 211, // 나룻배 (보물 찾기 1번 제목)
	TREASURE_BOAT_2 = 212, // 돛단배 (보물 찾기 2번 제목)
	TREASURE_BOAT_3 = 213, // 뿌까의 돛단배 (보물 찾기 3번 제목)
	TREASURE_BOAT_TEXT_1 = 214, // 나룻배를 타고 바다로! 돛이 없어서 멀리 못 갈 것 같아!  (보물 찾기 1번 설명)
	TREASURE_BOAT_TEXT_2 = 215, // 돛단배를 타고 바다로! 멀리 온 것 같은데 왜 힘들지?  (보물 찾기 2번 설명)
	TREASURE_BOAT_TEXT_3 = 216, // 뿌까의 돛단배를 타고 바다로! 수월한 보물찾기가 되겠어! (보물 찾기 3번 설명)
	ACCOUNT_LEVEL_OPEN = 217, // Lv.{0} 오픈 
	FINISH = 218, // 완료
	BATTLE_GIVEUP_TITLE = 219, // 전투 포기
	BATTLE_GIVEUP_DESC = 220, // 패배 처리되며 소모된 하트는\n돌려 받지 못합니다.\n전투를 종료하시겠습니까?
	SWEEP_CANT_USE = 221, // 현재 스테이지를 완료해야 소탕 기능을 사용할 수 있습니다.
	GUILD_INTRO_REVISE = 222, // 소개글 수정
	RANKING = 223, // 랭킹
	PERSON = 224, // 개인
	RANKING_SEASON_REWARD = 225, // 시즌보상
	RANKING_DAILY_REWARD = 226, // 일일보상
	RANKING_RANK_REWARD = 227, // 현재 순위 보상
	REWARD_MAIL_ACCEPT = 228, // 보상은 우편으로 자동 지급됩니다.
	RANKING_SEASON_REWARD_INFO = 229, // 보상은 시즌이 종료 된 후 우편으로 지급됩니다
	MORE = 230, // {0}점 이상
	RANKING_DAILY_REWARD_INFO = 231, // 보상은 매일 밤 자정에 우편으로 지급됩니다.
	RANKING_ACTIVITY_REWARD = 232, // 참여보상
	GULID_WHOLE_INFO = 233, // 길드 전체 정보
	MY_INFO = 234, // 내 정보
	GUILD_EMPTY = 235, // 소속된 길드가 없습니다.
	PLACING = 236, // {0}위
	RUBY_GIVE = 237, // 루비 {0}개 지급
	RUBY_OBTAIN = 238, // 루비 {0}개 획득
	GUILD_EXP = 239, // 길드 경험치
	MAIL = 240, // 우편
	MAIL_DATE_KEEP = 241, // 우편은 지정된 기간까지만 보관됩니다.
	ALL_GET = 242, // 모두받기
	GET = 243, // 받기
	MAIL_EMPTY = 244, // 보관중인 우편이 없습니다.
	MAIL_GET = 245, // 우편수령
	MAIL_GET_INFO = 246, // 우편을 수령했습니다.
	GOODS_RECEIVE = 247, // {0} {1}개 받음
	LATER = 248, // 남음
	MAIL_PAY_ERROR_REWARD = 249, // 결제 오류 보상
	MAIL_SEASON_RANKING_REWARD = 250, // 랭킹 {0}위 보상
	MAIL_DAILY_RANKING_REWARD = 251, // 랭킹 {0}점 보상
	MAIL_ACHIEVE_DAILY_REWARD = 252, // 반복 업적 보상
	MAIL_ACHIEVE_NORMAL_REWARD = 253, // 일반 업적 보상
	MAIL_MANAGE_REWARD = 254, // 운영 보상
	MAIL_EVENT_REWARD = 255, // 이벤트 보상
	MAIL_REWARD_ACCEPT = 256, // {0}을(를) 지급
	ACHIEVE = 257, // 업적
	ACHIEVE_DAILY = 258, // 반복 업적
	ACHIEVE_NORMAL = 259, // 일반 업적
	ACHIEVE_ING = 260, // 진행중
	ACHIEVE_RESTART_INFO = 261, // 반복 업적을 모두 달성 시 {0}시간 후에 다시 달성할 수 있습니다.
	ACHIEVE_RESTART = 262, // {0}후에 다시 달성할 수 있습니다.
	REWARD_RECEIVE = 263, // 해당 보상을 수령했습니다.
	MAIL_PROVIDE = 264, // 우편함으로 지급되었습니다.
	MAIL_FULL = 265, // 우편이 꽉 찼습니다. 우편을 비워주세요.
	EVENT = 266, // 이벤트
	SHOP_BOX_BUY_INFO = 267, // 상자는 구입 즉시 오픈됩니다.
	SHOP_HEART_TITLE = 268, // 하트 {0}
	SHOP_GOLD_TITLE = 269, // 골드 {0}
	SHOP_RUBY_TITLE = 270, // 루비 {0}
	ACHIEVECLEAR = 271, // 업적 달성
	ADD_COUNT = 272, //  외 {0}
	ITEM_LIMIT_BUY = 273, // {0}(으)로 {1}(을)를\n 구매하시겠습니까?\n (남은 수 : {2})
	ITEM_BUY = 274, // {0}(으)로 {1}(을)를\n 구매하시겠습니까?
	STRANGESHOP = 275, // 수상한 상점
	REMAINING = 276, // {0}후 초기화
	SHOP_LIMIT_BUY = 277, // 남은 수 : {0}
	STRANGE_NPC_HI01 = 278, // 히히 안녕~ 널 위해 준비했어~
	STRANGE_NPC_HI02 = 279, // 오늘 뭐 가져왔게~?
	STRANGE_NPC_YES01 = 280, // 훌륭한 선택이야!
	STRANGE_NPC_YES02 = 281, // 후회 없을꺼야~
	STRANGE_NPC_YES03 = 282, // 멋진데?
	STRANGE_NPC_SOLDOUT01 = 283, // 어라, 벌써 다 떨어졌네 내일을 기대해~
	STRANGE_NPC_NO01 = 284, // 뭐야, 부족해?
	SECRET_EXCHANGE = 285, // 비밀 거래
	EXCHANGE_TERMS = 286, // {0}등급 {1}장
	EXCHANGE = 287, // 거래
	CARD_INSERT = 288, // 카드 등록
	CARD_POSSIBLE_GET = 289, // 획득 가능 카드
	POSSIBLE_GET_INFO = 290, // 아래 카드 중 1개 획득 가능합니다.
	SECRET_EXCHANGE_INFO = 291, // 카드를 소모하여 거래를 진행하시겠습니까?
	CARD_DECK_GET = 292, // {0}에서 획득할 수 있습니다.
	SECRET_EXCHANGE_NPC_01 = 293, // 나랑 거래하러 왔어?
	SECRET_EXCHANGE_NPC_02 = 294, // 여긴 어떻게 찾아온거지?
	SECRET_EXCHANGE_NPC_03 = 295, // 여기에 뭐가 들었게~?
	SECRET_EXCHANGE_NPC_04 = 296, // 어떤 카드를 나한테 줄꺼야?
	SECRET_EXCHANGE_NPC_05 = 297, // 빨리 내놔!
	SECRET_EXCHANGE_NPC_06 = 298, // 좀 더 좋은 카드 없어?
	SECRET_EXCHANGE_NPC_07 = 299, // 난 저 카드를 원해
	SECRET_EXCHANGE_NPC_08 = 300, // 기대해도 좋아
	SECRET_EXCHANGE_NPC_09 = 301, // 좋아, 거래하지!
	REVENGEBATTLE = 302, // 복수전
	REVENGE = 303, // 복수
	REVENGEBATTLE_INFO = 304, // 현재 덱으로 복수를 진행하시겠습니까?
	DECK_EDIT = 305, // 덱 편집
	DECK_EDIT_BATTLE_INFO = 306, // 변경된 덱으로 복수를 진행하시겠습니까?
	NOT_REVENG_UESR = 307, // 공격해온 유저가 없습니다.
	REVENG_SUCCESS = 308, // 복수에 성공했습니다.
	REVENG_DRAW = 309, // 캐릭터들이 아쉬워하고 있습니다.
	BATTLE_RETRY = 310, // 재도전
	REVENG_FAIL = 311, // 캐릭터들이 분노하고 있습니다.
	SHORTCUT = 312, // 바로 가기
	SC_LOBBY = 313, // 로비
	SC_REVENGBATTLE = 314, // 복수전
	REVENGE_POINT = 315, // 복수 포인트
	RANK_POINT = 316, // 랭킹 점수
	WIN_CONTINUE_BONUS = 317, // {0}연승 보상
	WIN_BONUS = 318, // 승점
	WIN_BONUS_INFO = 319, // 매칭 승리 시 {0}점 획득하며, {1}점 도달 시 추가 보상을 획득합니다.
	WIN_CONTINUE_BONUS_INFO = 320, // {0}연승 중!
	GET_EXP = 321, // 획득한 경험치
	REPLAY = 322, // 다시하기
	NEXT_STAGE = 323, // 다음지역
	SC_CARD = 324, // 카드
	SC_GUILDINFO = 325, // 길드
	INGAME_MOB_INFO = 326, // 적들이 몰려옵니다!
	DIFFICULTY = 327, // 난이도 : 
	EASY = 328, // 쉬움
	NORMAL = 329, // 보통
	HARD = 330, // 어려움
	RECEIVE_EXP = 331, // 획득 경험치
	BOSS = 332, // BOSS
	DONATION_SUCCESS = 333, // 기부를 완료했습니다.
	DONATION_MSG = 334, // {0}{1}을(를) 기부하시겠습니까?
	GUILD_JOIN_MSG = 335, // {0} 길드에 가입 신청했습니다.
	GUILD_POINT_PLUS = 336, // 길드 포인트 + {0}
	GUILD_EXP_PLUS = 337, // 길드 경험치 + {0}
	NETWORK_FAILED = 338, // 서버와 통신에 실패했습니다.\n로그인 화면으로 돌아갑니다.
	MAKE_NAME = 339, // 닉네임생성
	CHECK = 340, // 체크
	POSSIBLE_NAME = 341, // 사용이가능한닉네임입니다.
	CVERLAP_NAME = 342, // 중복된닉네임입니다.
	BAN_NAME = 343, // 금지어 포함된 닉네임 입니다.
	MAKE_NAME_INFO = 344, // 최소2자최대8자까지입력가능\n특수문자,공백사용불가\n※확인후닉네임변경불가
	UPDATA_INFO = 345, // 파일 업데이트를 받고 있습니다 ({0}/{1})
	SCREEN_TOUCH = 346, // 화면을 터치해주세요
	ERROR = 347, // 오류
	DOWNLOAD = 348, // 다운로드
	OVERLAP_ACCESS = 349, // 중복접속
	RECONNECT = 350, // 재접속
	NETWORK_FAILED_TITLE = 351, // 서버와의연결시간을초과하였습니다.
	NOT_DATA = 352, // 통신 데이터 연결이 되어있지 않습니다.
	RECOMMEND_WIFI = 353, // 대용량 데이터를 받습니다.\n와이파이 사용을 권장합니다.
	NEW_VERSION = 354, // 새 버전이 있습니다.\n업데이트 해주시기 바랍니다.
	DIFFERENT_PHONE_ACCESS = 355, // 다른 기기에서 접속 중입니다.\n다른 기기의 접속을 끊고\n이 기기로 접속하시겠습니까?
	CHANGE_PHONE_LOSS = 356, // 게스트 계정은 게임 재설치, 기기 변경 등에 의해\n데이터가 사라질 가능성이 있습니다.\n그래도 진행하시겠습니까?\n(환경설정에서 계정연동이 가능합니다.)
	MAKE_NAME_IDENFIFY = 357, // 닉네임 : {0}\n시작하시겠습니까?
	CONTINUE = 358, // 계속
	CLICK_WRAP = 359, // 약관동의
	PERSONAL_INFO_COLLECTION = 360, // 개인정보수집
	TERMS_OF_SERVICE = 361, // 서비스 이용 약관
	AGREE = 362, // 동의합니다.
	FRANCHISE = 363, // 가맹점
	FLOOR = 364, // {0} {1}F
	FLOOR_INFO = 365, // {0}F 정보
	FRANCHISE_REWARD_INFO = 366, // 일정시간마다\n{0}{1}을(를) 획득합니다.
	TIME_RWQUIRED = 367, // 소요 시간
	FLOOR_OPEN_TERMS = 368, // 확장 조건
	ACCOUNT_LEVEL = 369, // 계정 레벨
	PRECEDE_TERMS = 370, // 선행 확장
	NEED_TERMS = 371, // 필요 재화
	FLOOR_OPEN = 372, // 확장
	FLOOR_OPEN_TERMS_INFO = 373, // {0}F을(를) 확장하시겠습니까?\n확장에 필요한 재화들이 소모됩니다.
	WRIT_NAME = 374, // 사용하실 닉네임을 입력해주세요.
	EMPTY_NAME = 375, // 닉네임을 입력해주세요.
	LACK_WORDS = 376, // 글자 수가 부족합니다.
	EXCESS_WORDS = 377, // 글자 수를 초과했습니다.
	TREASURE_DETECT = 378, // 보물수색
	ISLAND_TERRAPIN = 379, // 자라섬
	ISLAND_COCONUT = 380, // 야자섬
	ISLAND_ICE = 381, // 얼음섬
	ISLAND_LAKE = 382, // 호수섬
	ISLAND_BLACK = 383, // 검은섬
	ACCOUNT_LEVEL_INFO = 384, // 계정레벨 {0}부터 입장할 수 있습니다.
	TREASURE_MAP_NONE = 385, // {0} 보물지도가 없습니다.
	TREASURE_DETECT_INFO = 386, // {0}을 수색하시겠습니까?\n{0} 보물지도 {1}개가 소모됩니다.
	TREASURE_OK = 387, // 보물 확인
	TREASURE_DETECT_EXIT = 388, // 지도 화면으로 나가시겠습니까?\n소모된 보물지도는 복구되지 않습니다.
	FRANCHISE_EXPANSION_INFO_B01_1 = 389, // {1}을(를) 냄비에 넣고 휘저으면… 짜잔! {2}개가 되었습니다!
	FRANCHISE_EXPANSION_INFO_B01_2 = 390, // {1} {2}개 주문받았다 해. 기다리라 해. 쎼쎼!
	FRANCHISE_EXPANSION_INFO_B01_3 = 391, // 크~ {1}이(가)라니, 손님 보는 눈 있다 해. {2}개 조금만 기다리라 해!
	FRANCHISE_EXPANSION_INFO_B02_1 = 392, // 이랏샤이! 여기선 {1}을(를) {2}개씩 얻으실 수 있습니다!
	FRANCHISE_EXPANSION_INFO_B02_2 = 393, // 요리는 집중! {1} {2}개을(를) 얻으려면 그 정도는 기다려야지
	FRANCHISE_EXPANSION_INFO_B02_3 = 394, // 쉿, 지금이 제일 중요한 순간이야. {1}을(를) {2}개나 얻는 게 어디 쉬운 줄 아나?
	FRANCHISE_EXPANSION_INFO_B03_1 = 395, // 어서 오세요. 이 곳에서는 {1}을(를) {2}개 얻을 수 있답니다.
	FRANCHISE_EXPANSION_INFO_B03_2 = 396, // 어서오세요 몇 분이신가요? 여기 앉아서 기다리시면 {1} {2}개 금방 내오겠습니다.
	FRANCHISE_EXPANSION_INFO_B03_3 = 397, // {1} 말인가요? {2}개 정도라면 기다리셔야 합니다. 제법 귀하거든요.
	FRANCHISE_EXPANSION_INFO_B04_1 = 398, // 우리 가게는 {1}가 최고 인기지. {2}개나 주문받을 정도라고!
	FRANCHISE_EXPANSION_INFO_B04_2 = 399, // 열정적인 당신에게는 {1}가 딱일 걸? {2}개 정도라면 금방이지!
	FRANCHISE_EXPANSION_INFO_B04_3 = 400, // {1}을(를) 얻기는 꽤 어렵지. 사랑과 정열을(를) 가득 담아 열심히 조려내야 {2}개 정도가 나온다고!
	FRANCHISE_EXPANSION_INFO_B05_1 = 401, // 특별한 서비스! 이곳에서는 {1}을(를) {2}개 얻을 수 있습니다! 지금 선택하세요!
	FRANCHISE_EXPANSION_INFO_B05_2 = 402, // {1}콤보 {2}개 주문 받았습니다. 잠시만 기다려 주세요!
	FRANCHISE_EXPANSION_INFO_B05_3 = 403, // 주문 확인하겠습니다~ {1}셋트 {2}개 맞으시죠? 드시고 가실 건가요?
	OPTION_GAME_SETTING = 404, // 환경설정
	OPTION_GAME_INFO = 405, // 게임정보
	BGM = 406, // 배경음
	SFX = 407, // 효과음
	PUSH_NOTICE = 408, // 푸시 알림
	GAME_VERSION = 409, // 게임 버전
	MEMBER_NUMBER = 410, // 회원 번호
	COPY = 411, // 복사
	CUSTOMER_QUESTION = 412, // 고객문의
	COMMUNITY = 413, // 커뮤니티
	LINKED_ACCOUNT_CHECK = 414, // 연동계정 확인
	AGREEMENT = 415, // 이용약관
	COUPON_INPUT = 416, // 쿠폰입력
	ACCOUNT_LINK = 417, // 계정연동
	ACCOUNT_ESCAPE = 418, // 계정탈퇴
	LOG_OUT = 419, // 로그아웃
	COUPON_INPUT_TERM = 420, // 대소문자 구분없이 연속 입력
	COPY_FINISHED = 421, // 복사가 완료되었습니다.
	ACCOUNT_CHECK_LINKED = 422, // {0} 사용 중입니다.
	ACCOUNT_CHECK_UNLINKED = 423, // {0} 사용 중입니다.\n계정 연동을 추천합니다.
	LOG_OUT_CONFIRM = 424, // 로그아웃 하시겠습니까?\n초기 화면으로 이동합니다.
	ACCOUNT_SECESSION_WARNING = 425, // 탈퇴하시겠습니까?\n탈퇴 시 모든 데이터가 삭제되며\n복구가 불가능합니다.\n계속하시려면 'puccawarsbye'\n를 입력하세요.
	ACCOUNT_SECESSION_ERROR_TYPING = 426, // 잘못된 입력입니다.\n다시 확인 후 입력해주세요.
	ACCOUNT_GUEST = 427, // 게스트 계정
	ACCOUNT_GOOGLE_PLUS = 428, // 구글 플러스 계정
	COUPON_ERROR = 429, // 쿠폰 오류입니다.\n[{0}]
	COUPON_ERROR_USED_NUMBER = 430, // 사용된 쿠폰 번호
	COUPON_ERROR_TIME_EXPIRED = 431, // 기간 만료
	COUPON_ERROR_WRONG_NUMBER = 432, // 잘못된 쿠폰 번호
	ACCOUNT_LINKED_FINISHED = 433, // 연동이 완료되었습니다.
	LANGUAGE = 434, // 언어
	LANGUAGE_RESTART = 435, // 언어 적용을 위해 재시작 하시겠습니까?
	WIN_CONTINUE_BONUS_MSG = 436, // 2연승부터 별을 획득합니다.
	GOODS_REVENGE_POINT = 437, // 복수 포인트
	GOODS_SMILE_POINT = 438, // 스마일 포인트
	GOODS_TREASUREDETECTMAP_TERRAPIN = 439, // 자라섬 보물지도
	GOODS_TREASUREDETECTMAP_COCONUT = 440, // 야자섬 보물지도
	GOODS_TREASUREDETECTMAP_ICE = 441, // 얼음섬 보물지도
	GOODS_TREASUREDETECTMAP_LAKE = 442, // 호수섬 보물지도
	GOODS_TREASUREDETECTMAP_BLACK = 443, // 검은섬 보물지도
	GOODS_SWEEP_TICKET = 444, // 소탕권
	DEVICE_ANGLE = 445, // {0}˚
	DEVICE_ANGLE_NOT_AVAILABLE = 446, // N/A
	RETURN = 447, // 복귀
	PAUSE_DESC = 448, // 일시 정지 중입니다.
	NOT_ACCESS_LONG_SCREEN_RESTORE = 449, // 오랫동안 접속하지 않아\n초기 화면으로 돌아갑니다.
	SKILL = 450, // 스킬
	SKILL_NORMAL = 451, // 일반
	NEXT_AREA = 452, // 다음 지역으로!
	NEXT_AREA_MSG = 453, // 랭킹점수 상승으로\n{0}(으)로 이동합니다.
	BEFORE_AREA = 454, // 이전 지역으로!
	BEFORE_AREA_MSG = 455, // 랭킹점수 하락으로\n{0}(으)로 이동합니다.
	NORMAL_SHOP_BOX_CONTENTS = 456, // {0}{1}(으)로 {2}을(를) 구매하시겠습니까?
	NORMAL_SHOP_PACKAGE_CONTENTS = 457, // {0}{1}(으)로 {2}을(를) 구매하시겠습니까?
	CARD_GRADE_FIXED_GET = 458, // {0}등급 카드 확정 획득
	GOODS_CARD_COUNT = 459, // {0} {1}장
	GOODS_COUNT = 460, // {0} {1}개
	BUTTON_INFORMATION = 461, // 정보
	LOGIN_ERROR_NETWORK = 462, // 네트워크 오류가 발생했습니다.\n로그인 화면으로 돌아갑니다.
	LOGIN_ERROR_LOGIN = 463, // 로그인 오류가 발생했습니다.\n로그인 화면으로 돌아갑니다.
	SC_TREASURE_DETECT = 464, // 보물수색
	SC_FRANCHISE = 465, // 가맹점
	SC_ADVENTURE = 466, // 모험
	SC_SHOP = 467, // 상점
	MAKING_SUPPORT = 468, // 제작지원
	PUCCA_PROJECT_SUPPORT_GNEXT = 469, // 본 프로젝트는\nG-Next <2016 경기 차세대 게임 개발 지원 사업>\n의 지원을 받아 제작되었습니다.
	SALES_RAMNANT_TIME = 470, // 판매 종료 시각
	BUY_LIMIT_ACCOUNT_LEVEL = 471, // {0}레벨까지 구매 가능
	BUY_LIMIT_COUNT = 472, // 남은 구매 횟수 {0}
	CAN_NOT_BUY_ACCOUNT_LEVEL = 473, // 계정레벨이 맞지 않아 구매할 수 없습니다.
	CAN_NOT_BUY_COUNT_LIMIT = 474, // 더 이상 구매할 수 없습니다.
	SECOND = 475, // 초
	TREASURE_DETECT_DETECTING = 476, // 근처에 상자가 있습니다.\n탐색 중이니 그대로 유지하세요.
	TREASURE_FIND = 477, // 상자를 찾았습니다!\n상자를 조준하고 화면을 터치해서 열어보세요.
	TREASURE_DETECT_START = 478, // 주변을 둘러보며 상자를 찾아보세요.
	ACCOUNT_SP_FACEBOOK = 479, // Facebook
	ACCOUNT_SP_APPLE = 480, // Apple
	ACCOUNT_GUESTEDITER = 481, // GuestEditer
	CLASS_HEALER = 482, // 치유사
	CLASS_HITTER = 483, // 타격대
	CLASS_KEEPER = 484, // 수호자
	CLASS_RANGER = 485, // 공격수
	CLASS_WIZARD = 486, // 도사
	CHARACTER_NAME = 487, // 캐릭터명
	QUESTION_CONTENTS = 488, // 문의내용
	PIECE = 489, // 개
	EXPANSION = 490, // 확장
	SWEEP_COUNT_REMAIN_TODAY = 491, // 오늘의 남은 소탕횟수
	BUFF_SKILL_EXPLANATION_LIST = 492, // 버프 스킬 설명 목록
	NOT_YET_AVAILABLE = 493, // 아직 사용할 수 없습니다.
	MASTER_SOO_FOLLOWER = 494, // 수노인의 추종자
	SHOP_TIME_UTC = 495, // ~{0}-{1}-{2}\nT{3}{4}
	SHOP_BOX_DROP_AREA = 496, // 매칭 지역에 해당하는 카드를 획득합니다.
	MAIL_SHOP_PACKAGE = 497, // {0} 구매
	GOODS_CARD = 498, // 카드
	GOODS_BOX = 499, // 상자
	FRANCHISE_CHINA = 500, // 중식당
	FRANCHISE_KOREA = 501, // 한식당
	FRANCHISE_JAPEN = 502, // 일식당
	FRANCHISE_ITALY = 503, // 이태리 식당
	FRANCHISE_FASTFOOD = 504, // 패스트푸드점
	NOTICE_MAIN_TITLE = 505, // Notice
	NOTICE_SERVER_CLOCK = 506, // {0}-{1}-{2}T{3}:{4}:{5}
	NOTICE_TITLE_TAG_SYSTEM = 507, // System
	NOTICE_TITLE_TAG_EVENT = 508, // Event
	NOTICE_TITLE_TAG_PROMOTION = 509, // Promotion
	NOTICE_TITLE_TYPE_HOT = 510, // Hot
	NOTICE_TITLE_TYPE_NEW = 511, // New
	NOTICE_OK = 512, // OK
	CASH_PACKAGE_NAME_01 = 513, // 오픈 패키지 1
	CASH_PACKAGE_NAME_02 = 514, // 오픈 패키지 2
	CASH_PACKAGE_NAME_03 = 515, // 초급 패키지
	CASH_PACKAGE_NAME_04 = 516, // 카드 패키지 1
	CASH_PACKAGE_NAME_05 = 517, // 카드 패키지 2
	CASH_PACKAGE_NAME_06 = 518, // 성장 패키지 1
	CASH_PACKAGE_NAME_07 = 519, // 성장 패키지 2
	CASH_PACKAGE_NAME_08 = 520, // 마스터 패키지 1
	CASH_PACKAGE_NAME_09 = 521, // 마스터 패키지 2
	CASH_PACKAGE_BOX_NAME_01 = 522, // 오픈 패키지 상자 1
	CASH_PACKAGE_BOX_NAME_02 = 523, // 오픈 패키지 상자 2
	CASH_PACKAGE_BOX_NAME_03 = 524, // 초급 패키지 상자
	CASH_PACKAGE_BOX_NAME_04 = 525, // 카드 패키지 상자 1
	CASH_PACKAGE_BOX_NAME_05 = 526, // 카드 패키지 상자 2
	CASH_PACKAGE_BOX_NAME_06 = 527, // 마스터 패키지 상자 1
	CASH_PACKAGE_BOX_NAME_07 = 528, // 마스터 패키지 상자 2
	SHOP_TAB_CASH_PACKAGE = 529, // 패키지
	SC_DISABLED_ICON = 530, // {0}레벨에 해제됩니다.
	MAIL_REMAIN_INFINITE = 531, // 무제한
	ITEM_BUY_NO_ENTER = 532, // {0}(으)로 {1}을(를) 구매하시겠습니까?
	CASH_RUBY_BUNDLE_NAME_01 = 533, // 루비 x20
	CASH_RUBY_BUNDLE_NAME_02 = 534, // 루비 x61
	CASH_RUBY_BUNDLE_NAME_03 = 535, // 루비 x210
	CASH_RUBY_BUNDLE_NAME_04 = 536, // 루비 x660
	CASH_RUBY_BUNDLE_NAME_05 = 537, // 루비 x1150
	CASH_RUBY_BUNDLE_NAME_06 = 538, // 루비 x2400
	DETECT_MANUAL = 539, // 직접 수색
	DETECT_AUTO = 540, // 자동 수색
	NOT_SUPPORT_DEVICE = 541, // 지원하는 기기가 아닙니다.
	NOT_ENOUGH_TERMS = 542, // 조건이 부족합니다.
	MAIL_REVENGEBATTLE_REWARD = 543, // 복수전 보상
	DETECT_CANCEL = 544, // 수색 취소
	DETECT_AUTO_FINISH_TIME = 545, // 완료까지 남은시간 {0}초
	DETECT_AUTO_INFO = 546, // 수색이 완료되면 보물 상자가 자동으로 열립니다.
	DEVICE_PUSH_NOTIBAR_TITLE_PLAY = 547, // 달려봅시다!
	DEVICE_PUSH_NOTIBAR_TITLE_NORMAL = 548, // 똑똑 뿌까가 왔어요
	DEVICE_PUSH_NOTIBAR_TITLE_REWARD = 549, // 앗! 확인할 시간이에요
	DEVICE_PUSH_NOTIBAR_TITLE_SHOP = 550, // 새로운게 들어왔어요
	DEVICE_PUSH_NOTIBAR_CONTENT_TREASURE = 551, // 보물찾은 배가 도착했어요. 확인해보세요.
	DEVICE_PUSH_NOTIBAR_CONTENT_REQUEST = 552, // 길드 카드 요청 가능합니다. 확인해보세요.
	DEVICE_PUSH_NOTIBAR_CONTENT_HEART = 553, // 하트 충전이 완료되었습니다. 지금 접속해볼까요?
	DEVICE_PUSH_NOTIBAR_CONTENT_REWARD = 554, // {0} 보상 획득 가능합니다. 확인해보세요.
	DEVICE_PUSH_NOTIBAR_CONTENT_SHOP = 555, // {0}에 새로운 상품들이 들어왔어요. 확인해보세요.
	APPLY = 556, // 적용
	DEVICE_PUSH_TREASURE_REWARD = 557, // 보물찾기 보상
	DEVICE_PUSH_HEART_CHARGE = 558, // 하트 충전
	DEVICE_PUSH_CARD_REQUEST = 559, // 길드 카드 요청
	DEVICE_PUSH_FRANCHISE_REWARD = 560, // 가맹점 보상
	DEVICE_PUSH_SHOP_RENEW = 561, // 상점 갱신
	DEVICE_PUSH_RANKING_REWARD = 562, // 랭킹 보상
	DEVICE_PUSH_SETUP = 563, // 푸시 알림 설정
}

public enum SOUND_TYPE
{
	BGM = 1, // BGM
	SFX = 2, // SFX
}

public enum SOUND
{
	BGM_LOBBY = 1, // BGM_LOBBY
	BGM_EXPLORE = 2, // BGM_EXPLORE
	BGM_BATTLE_0 = 3, // BGM_BATTLE_0
	BGM_OCTO = 4, // BGM_OCTO
	SFX_BOOM_0 = 5, // SFX_BOOM_0
	SFX_DAMAGE_0 = 6, // SFX_DAMAGE_0
	SFX_FLAME = 7, // SFX_FLAME
	SFX_HEAL = 8, // SFX_HEAL
	SFX_SKILL_ACT = 9, // SFX_SKILL_ACT
	SND_CANCEL = 10, // SND_CANCEL
	SND_OK = 11, // SND_OK
	SFX_BUFF_0 = 12, // SFX_BUFF_0
	SFX_SKILL_FIST = 13, // SFX_SKILL_FIST
	SFX_THROW_0 = 14, // SFX_THROW_0
	SND_BUTTON_GOOD_0 = 15, // SND_BUTTON_GOOD_0
	SND_BUTTON_GOOD_1 = 16, // SND_BUTTON_GOOD_1
	SFX_BOMB = 17, // SFX_BOMB
	SFX_DAMAGE_1 = 18, // SFX_DAMAGE_1
	SFX_DEBUFF_0 = 19, // SFX_DEBUFF_0
	SFX_BICHEOLJO = 20, // SFX_BICHEOLJO
	SFX_SKILL_FLYINGAXE = 21, // SFX_SKILL_FLYINGAXE
	SFX_SKILL_HELLGRASS = 22, // SFX_SKILL_HELLGRASS
	SFX_SKILL_LASER = 23, // SFX_SKILL_LASER
	SFX_SKILL_LIGHTNING = 24, // SFX_SKILL_LIGHTNING
	SFX_SKILL_MISSILE = 25, // SFX_SKILL_MISSILE
	SFX_SKILL_POISON = 26, // SFX_SKILL_POISON
	SFX_SKILL_SHORKWAVE = 27, // SFX_SKILL_SHORKWAVE
	SFX_SKILL_SWORD = 28, // SFX_SKILL_SWORD
	SFX_SKILL_WEIGHT = 29, // SFX_SKILL_WEIGHT
	SFX_SKILL_WIND_B = 30, // SFX_SKILL_WIND_B
	SFX_SKILL_WIND_S = 31, // SFX_SKILL_WIND_S
	SFX_THROW_1 = 32, // SFX_THROW_1
	SFX_SKILL_SCRATCH = 33, // SFX_SKILL_SCRATCH
	SFX_SKILL_HEAVY_DROP_EXPLOSION = 34, // SFX_SKILL_HEAVY_DROP_EXPLOSION
	SFX_SKILL_HEAVY_DROP_LANDING = 35, // SFX_SKILL_HEAVY_DROP_LANDING
	SFX_SKILL_HEAVY_GLASS_BREAK = 36, // SFX_SKILL_HEAVY_GLASS_BREAK
	SFX_SKILL_NUNCHAKU = 37, // SFX_SKILL_NUNCHAKU
	SFX_SKILL_RANDOM_FAIL = 38, // SFX_SKILL_RANDOM_FAIL
	SFX_SKILL_RANDOM_SUCCESS = 39, // SFX_SKILL_RANDOM_SUCCESS
	SFX_SKILL_BURNNING = 40, // SFX_SKILL_BURNNING
	SFX_SKILL_FLASH = 41, // SFX_SKILL_FLASH
	SFX_SKILL_SPEED_UP = 42, // SFX_SKILL_SPEED_UP
	SFX_SKILL_SUMMON = 43, // SFX_SKILL_SUMMON
	SFX_SKILL_CAST_AURA = 44, // SFX_SKILL_CAST_AURA
	SFX_SKILL_CAST_BULLET_SHOT = 45, // SFX_SKILL_CAST_BULLET_SHOT
	SFX_SKILL_CAST_HEAVY_DROP = 46, // SFX_SKILL_CAST_HEAVY_DROP
	SFX_SKILL_CAST_RUN = 47, // SFX_SKILL_CAST_RUN
	SFX_SKILL_CAST_RUNS = 48, // SFX_SKILL_CAST_RUNS
	SFX_SKILL_CAST_SWORD_SLASH = 49, // SFX_SKILL_CAST_SWORD_SLASH
	SFX_SKILL_CAST_WIELD_HAMMER = 50, // SFX_SKILL_CAST_WIELD_HAMMER
	SFX_SKILL_CAST_WIELD_PUNCH = 51, // SFX_SKILL_CAST_WIELD_PUNCH
	SFX_SKILL_HIT_EXPLOSION_1 = 52, // SFX_SKILL_HIT_EXPLOSION_1
	SFX_SKILL_HIT_EXPLOSION_2 = 53, // SFX_SKILL_HIT_EXPLOSION_2
	SFX_SKILL_HIT_FREEZE = 54, // SFX_SKILL_HIT_FREEZE
	SFX_SKILL_HIT_PUNCH = 55, // SFX_SKILL_HIT_PUNCH
	SFX_SKILL_HIT_TELEPORT = 56, // SFX_SKILL_HIT_TELEPORT
	SND_UI_ACCOUNT_LEVELUP = 57, // SND_UI_ACCOUNT_LEVELUP
	SND_UI_ACHIEVEMENT_COMPLETED = 58, // SND_UI_ACHIEVEMENT_COMPLETED
	SND_UI_ACHIEVEMENT_REWARD = 59, // SND_UI_ACHIEVEMENT_REWARD
	SND_UI_CARD_ACT_01 = 60, // SND_UI_CARD_ACT_01
	SND_UI_CARD_ACT_02 = 61, // SND_UI_CARD_ACT_02
	SND_UI_CHESTBOX_ACT = 62, // SND_UI_CHESTBOX_ACT
	SND_UI_FRANCHISE_GETITEM = 63, // SND_UI_FRANCHISE_GETITEM
	SND_UI_GUILD_SEND = 64, // SND_UI_GUILD_SEND
	SND_UI_PVP_PVE_RESULT_CLEAR = 65, // SND_UI_PVP_PVE_RESULT_CLEAR
	SND_UI_PVP_PVE_RESULT_FAIL = 66, // SND_UI_PVP_PVE_RESULT_FAIL
	SND_UI_PVP_RESULT_BOX = 67, // SND_UI_PVP_RESULT_BOX
	SND_UI_PVP_RESULT_STARPOINT = 68, // SND_UI_PVP_RESULT_STARPOINT
	SND_UI_PVP_RESULT_WINPOINT_REWARD = 69, // SND_UI_PVP_RESULT_WINPOINT_REWARD
	SND_UI_SECRETEXCHANGE_READY = 70, // SND_UI_SECRETEXCHANGE_READY
	SND_UI_TREASUREDETECT_BOX = 71, // SND_UI_TREASUREDETECT_BOX
	SND_UI_UPGRADE = 72, // SND_UI_UPGRADE
	SND_UI_CANCEL = 73, // SND_UI_CANCEL
	SND_UI_OK = 74, // SND_UI_OK
}

public enum BATTLE_BULLET_TYPE
{
	HORIZON = 1, // 전투투사체_일반직선이동
	MAGIC_TARGET_ATT = 2, // 마법_대상에게 직접 공격
	MAGIC_TARGET_HEAL = 3, // 마법_대상에게 힐
}

public enum BUFF_KIND
{
	DOT_DAMAGE = 1, // 지속데미지
	KNOCKBACK = 2, // 밀어내기
	AIRBORN = 3, // 띄우기
	SILENCE = 4, // 침묵
	ANTI_DEBUFF = 5, // 해로운효과 제거
	FREEZE = 6, // 행동불능
	ATT_UP = 7, // 공격력 증가
	DEF_UP = 8, // 방어력 증가
	ATT_DOWN = 9, // 공격력 감소
	DEF_DOWN = 10, // 방어력 감소
	SLOW = 11, // 모든 속도 감소
	ACCURATE_UP = 12, // 명중률 증가
	ACCURATE_DOWN = 13, // 명중률 감소
	EVADERATE_UP = 14, // 회피율 증가
	EVADERATE_DOWN = 15, // 회피율 감소
	CRITICALRATE_UP = 16, // 치명타율 증가
	CRITICALRATE_DOWN = 17, // 치명타율 감소
	CRITICALDMG_UP = 18, // 치명 피해량 증가
	CRITICALDMG_DOWN = 19, // 치명 피해량 감소
	SKILL_SHIELD = 20, // 스킬 데미지를 경감하고 소멸
	HARD_TANKING = 21, // 모든 데미지를 혼자 감당한다
	HEALING_ONE = 22, // 1회 HP 회복
	HEALING_CONSIST = 23, // 지속 HP 회복
	FAST = 24, // 이속/공속 증가
	GET_COST = 25, // 마나획득
	LOST_HP = 26, // 체력 감소/%
	TOTEM_HEALING_CONSIST = 27, // 토템-생존중에 회복
	BERSERKER_ATT_UP = 28, // 광전사용 공격력 증가
	BERSERKER_DP_DOWN = 29, // 광전사용 방어력 감소
	POISON = 30, // 독
	STUN = 31, // 기절
	CHARGING_UP = 32, // 밀어내기 증가.
	SACRIFICE = 33, // 희생. 지속 데미지 입는다.
	REFLECT = 34, // 데미지 반사.
	REMOVE_ICE = 35, // 빙결 해제.
	REMOVE_SUMMON = 36, // 소환물 삭제.
	REMOVE_BUFF = 37, // 이로운 버프효과 삭제.
	STATIC_DAMAGE = 38, // 고정데미지.
	DEF_IGNORE = 39, // 방어무시데미지
	DEF_IGNORE_DOT = 40, // 방어무시 도트데미지
	COOLTIME_DOWN = 41, // 쿨타임 감소
}

public enum ShortCutType
{
	None = 1, // 숏컷 타입 Null 값
	ShortCut_Treasure = 2, // 보물찾기 (숏컷 타입)
	ShortCut_Achieve = 3, // 업적 (숏컷 타입)
	ShortCut_Collection = 4, // 도감 (숏컷 타입)
	ShortCut_Ranking = 5, // 랭킹 (숏컷 타입)
	ShortCut_Option = 6, // 환경설정 (숏컷 타입)
	ShortCut_Lobby = 7, // 로비 (숏컷 타입)
	ShortCut_StrangeShop = 8, // 수상한 상점 (숏컷 타입)
	ShortCut_SecretExchange = 9, // 비밀거래 (숏컷 타입)
	ShortCut_RevengeBattle = 10, // 복수전 (숏컷 타입)
	ShortCut_Card = 11, // 카드 (숏컷 타입)
	ShortCut_GuildInfo = 12, // 길드 (숏컷 타입)
	ShortCut_Treasure_Detect = 13, // 보물수색 (숏컷 타입)
	ShortCut_Franchise = 14, // 가맹점 (숏컷 타입)
	ShortCut_Adventure = 15, // 모험 (숏컷 타입)
	ShortCut_ShopP = 16, // 상점 (숏컷 타입)
}

public enum SKILL_TARGET_TYPE
{
	NONE = 1, // 논 타겟
	AREA = 2, // 지역타격
	SELF = 3, // 자신에게
	TEAM_ALL = 4, // 팀전체
	RANDOM = 5, // 아무에게나
	FIRST_KEEPER = 6, // 수호자우선
	FIRST_HITTER = 7, // 타격대우선
	FIRST_RANGER = 8, // 레인저우선
	FIRST_HEALER = 9, // 힐러우선
	FIRST_WIZARD = 10, // 도사우선
	HIGH_HP = 11, // 높은체력
	LOW_HP = 12, // 낮은체력
	HIGH_AP = 13, // 높은공격력
	LOW_AP = 14, // 낮은공격력
	HIGH_DP = 15, // 높은방어력
	LOW_DP = 16, // 낮은방어력
}

public enum SKILL_ALLY_TYPE
{
	NONE = 1, // 없음
	ALLY = 2, // 아군
	ENEMY = 3, // 적군
}

public enum SKILLACTIVE_ACTION
{
	NONE = 1, // 없음
	MY_HIT_COUNT = 2, // 내가 맞은 횟수
	MYTEAM_HIT_COUNT = 3, // 우리팀이 맞은횟수
	TARGET_HIT_COUNT = 4, // 대상이 맞은횟수
	ENEMYTEAM_HIT_COUNT = 5, // 적팀이 맞은횟수
	MY_MISS_COUNT = 6, // 회피횟수
	MYTEAM_MISS_COUNT = 7, // 우리팀 미스 회수
	TARGET_MISS_COUNT = 8, // 대상 미스 회수
	ENEMYTEAM_MISS_COUNT = 9, // 적팀 미스 회수
	MYTEAM_TOTAL_HP = 10, // 우리팀 체력
	ENEMYTEAM_TOTAL_HP = 11, // 적팀 체력
	TIME = 12, // 시간
}

public enum SKILLACTIVE_VALUETYPE
{
	NONE = 1, // 액티브스킬 변수 Null 값
	INTEGER = 2, // 정수형
	RATE = 3, // 실수형
}

public enum SKILL_EFFECT_TYPE
{
	NONE = 1, // 스킬 이펙트 Null 값
	BULLET_NONETARGET = 2, // 던지면 대상없이 날아가는 투사체
	BULLET_HOMING_NORMAL = 3, // 던지고 따라가서 히트
	BULLET_HOMING_MAKE = 4, // 던지고 따라가서 히트 후 뭔가를 생성
	MAGIC = 5, // 마법
	TARGET = 6, // 대상
	SUMMON_SKY = 7, // 소환물_하늘
	SUMMON_HOMING = 8, // 소환물_하늘에서타겟유도
	SUMMON_GROUND = 9, // 소환물_바닥
}

public enum SKILL_KIND
{
	NONE = 1, // 스킬 종류 Null 값
	BUFF = 2, // 스킬유형-버프
	DAMAGE = 3, // 스킬유형-데미지
	HEAL = 4, // 스킬유형-회복
	SLOW = 5, // 이속/공속 감소
	FAST = 6, // 이속/공속 증가
	SPECIAL = 7, // 특수액션
}

public enum STAGE_TYPE
{
	NORMAL = 1, // 일반 스테이지
	SPECIAL = 2, // 중간보스 스테이지
	BOSS = 3, // 보스 스테이지
}

public enum Rank_Type
{
	None = 1, // 랭크 종류 Null 값
	Rate = 2, // 비율
	Rank = 3, // 순위 절대값
}

public enum Achieve_Type
{
	None = 1, // 업적 종류 Null 값
	Normal_Type = 2, // 일반
	Battle_Type = 3, // 전투
	Character_Type = 4, // 캐릭터
}

public enum Enemy_Type
{
	NORMAL = 1, // 일반 몬스터
	BOSS = 2, // 보스 몬스터
}

public enum Const_IndexID
{
	Const_Revenge_Enter_Price = 1, // 복수전 입장 비용 (하트)
	Const_Revenge_Point_Reward = 2, // 복수전 복수포인트 획득량
	Const_Revenge_Match_List = 3, // 복수전 기록 목록 수
	Const_Achieve_Last_Step = 4, // 업적 최종 단계
	Const_Achieve_Daily_Reset_Cycle_Hour = 5, // 업적 일일 초기화 주기 (시)
	Const_Achieve_Daily_Number = 6, // 업적 일일 수
	Const_Ranking_Reset_Cycle_Day = 7, // 랭킹 시즌 초기화 주기
	Const_Ranking_Page_Count = 8, // 랭킹 페이지 수
	Const_Ranking_List_Per_Page = 9, // 랭킹 페이지 당 목록 수
	Const_Account_Level_Limit = 10, // 계정 레벨 제한
	Const_Heart_Default_Limit = 11, // 하트 기본 수 제한
	Const_Heart_Recovery_Cycle_Sec = 12, // 하트 회복 주기 (초)
	Const_Guild_Making_Cost_Gold = 13, // 길드 생성 비용
	Const_Guild_Member_Limit = 14, // 길드 인원 제한
	Const_Guild_Accept_Wait_Member_Limit = 15, // 길드 가입 신청자 수 (가입 전)
	Const_Guild_Name_Length_Limit = 16, // 길드 이름 길이 제한
	Const_Guild_Introduce_Length_Limit = 17, // 길드 소개 길이 제한
	Const_Guild_Join_Apply_Limit = 18, // 길드 가입 신청 최대 길드 수 (가입 전)
	Const_Guild_Card_Receive_Limit = 19, // 길드 내가 요청한 카드를 받는 수 제한
	Const_Guild_Card_Request_Cycle_Sec = 20, // 길드 카드 요청 주기 (초)
	Const_Guild_Card_C_Support_Limit = 21, // 길드 남이 요청한 카드에 C등급 지원 수 제한
	Const_Guild_Card_B_Support_Limit = 22, // 길드 남이 요청한 카드에 B등급 지원 수 제한
	Const_Guild_Card_A_Support_Limit = 23, // 길드 남이 요청한 카드에 A등급 지원 수 제한
	Const_Guild_Chat_List_Limit = 24, // 길드 채팅 목록 수 제한
	Const_Guild_Chat_Character_Limit = 25, // 길드 채팅 글자 제한
	Const_Guild_Donate_Limit = 26, // 길드 기부 횟수 제한
	Const_Skill_Level_Limit = 27, // 스킬 레벨 제한
	Const_Equipment_Level_Limit = 28, // 장비 레벨 제한
	Const_Card_Level_Limit = 29, // 카드 레벨 제한
	Const_Battlepower_Value_Hp = 30, // 전투력 공식 상수 - 체력
	Const_Battlepower_Value_Ap = 31, // 전투력 공식 상수 - 공격력
	Const_Battlepower_Value_Dp = 32, // 전투력 공식 상수 - 방어력
	Const_Battlepower_Value_Skill = 33, // 전투력 공식 상수 - 스킬
	Const_Post_Refresh_Cycle_Sec = 34, // 우편 갱신 시간
	Const_Post_Keep_Day = 35, // 우편 일반 우편 유지 시간 (일)
	Const_Post_Notice_Keep_Day = 36, // 우편 공지 우편 유지 시간 (일)
	Const_Post_List = 37, // 우편 목록 수
	Const_Franchise_Smilepoint_Refresh_Cycle_Sec = 38, // 가맹점 스마일포인트 생성 주기 (초)
	Const_Franchise_Smilepoint_Receive_Count = 39, // 가맹점 스마일포인트 획득 수
	Const_Franchise_Building_Count = 40, // 가맹점 총 건물 수
}

public enum LanguageCode
{
	Unknown = 1, // http://timtrott.co.uk/culture-codes/
	Korean = 2, // Korea, Korean
	English = 3, // United States of America, English
	Chinese = 4, // China, Chinese
	ChineseTraditional = 5, // China, Traditional Chinese
	Japanese = 6, // Japan, Japanese
	French = 7, // France, French
	Vietnamese = 8, // Vietnam, Vietnamese
	Indonesian = 9, // Indonesia, Indonesian
	Portuguese = 10, // Portugal, Portuguese
	German = 11, // Germany, German
	Spanish = 12, // Spain, Spanish
	Russian = 13, // Russia, Russian
	Turkish = 14, // Turkey, Turkish
	Thai = 15, // Thailand, Thai
}

public enum TUTORIAL_TYPE
{
	TUTORIAL_TALK = 1, // 튜토리얼 대사
	TUTORIAL_HIGHLIGHT = 2, // 튜토리얼 버튼강조
	TUTORIAL_EXPLAIN = 3, // 튜토리얼 설명창
	TUTORIAL_WAIT = 4, // 튜토리얼 대기상태
	TUTORIAL_OPEN_CONTENTS = 5, // 튜토리얼 기능오픈
	TUTORIAL_END = 6, // 튜토리얼 종료.
	TUTORIAL_TIMEWAIT = 7, // 튜토리얼 시간대기.
}

public enum ItemProductType
{
	Single = 1, // 상품 종류 : 단품
	Package = 2, // 상품 종류 : 패키지
}

