# Bridge Mercenaries

## 플레이 타임 
- 10분

## 역할
- 캐릭터, 구조물 (박훈, 안도현, 정광민)
	- 애니메이션, 움직임, 로직, 스텟처리  
- 게임 매니저(박훈)

- 배경
	- Art

- 미니언 (안도현)
	- 로직, 애니메이션, AI

- Sound

- UI (나해인)

- 서버 (킹갓엠페러제네럴충무공김진철)

- 기획 (정현우)

- 이펙트 (정민규)

- 에셋 찾기 (정민규)

## 게임 모드
#### 1. 롤처럼<br>
양 끝에 핵을 두고 핵과 중앙의 가운데 부분에 포탑을 설치
중앙 양 사이드와 포탑 근처에 밖에서 안 보이는 공간을 만듬

#### 2. 눈치게임<br>
중앙에 핵을 둠<br>
핵은 미니언을 때림<br> 
플레이어가 핵을 때리면 내구도가 고정수치로 깎임<br>
일정량의 미니언을 모으면 일정 피해를 주고 만약 핵의 피가 그 피해보다 낮으면 데미지가 무시됨.<br>
마지막으로 때린 플레이어팀이 승리.<br>
돈으로 캐릭터 스텟을 키움<br>

#### 3. 파멸의탑<br>
특정 시간마다 점령지가 열리고 일정량의 게이지를 모으면 핵을 공격하는 이벤트를 발생한다.
	
	-스탯
	
		- 체력  
		- 마나  
		- 뎀지  
		- 이속  
		- 공속  

	- 구조물
		- 근거리, 원거리 병사
		- 핵

	- 구조물 설명
		- 근거리, 원거리 병사: 한 웨이브당 4마리로 2:2 비율 생성주기는 15초~20초(추후 조정)  
		- 핵: 미니언을 때림,



## 이벤트 (다양한 이벤트 중 랜덤으로)
1. 진 팀에게 페널티를 주자.
2. 이긴 팀에게 쿨타임 감소효과를 주자
3. 이긴 팀의 미니언에게 강화 버프를 주자.

## 배경
여러 시대에서 온 용병들이 자기 세계로 돌아가는 포탈을 열기 위해 마력이 있는 수정을 두고 싸움

## 맵 컨셉
강 위의 돌다리(중세 느낌)

## 핵
다리 중앙에 박힌 수정

## 시스템

#### 매치
- 메인화면 -> 게임시작 -> 1v1, 2v2 선택 ->  매치메이킹 -> 캐릭터 선택<br>

#### 캐릭터<br>
- 체력: 스탯 포인트 1마다 현재 체력의 10% 체력과 초당 4의 체력 재생을 얻음<br>
- 자원: 스탯 포인트 1마다 10%의 자원과 자원 재생을 얻음<br>
- 공격력: 스탯 포인트 1마다 스킬의 데미지가 스킬의 계수만큼, 기본 공격력은 20%만큼 오름<br>
- 이동속도: 스탯 포인트 1마다 5%의 이동속도가 추가됨<br>
- 공격속도: 스탯 포인트 1마다 15%의 공격속도를 얻음<br>
- 스탯의 최대치는 10<br>
- 캐릭터가 죽으면 죽인 상대는 죽은 캐릭터의 레벨x50 의 경험치를 얻음<br>

#### 레벨<br>
- 레벨업마다 2의 스탯 포인트가 주어짐<br>
- 레벨업에 필요한 경험치는 100+(레벨-1)x75<br>
- 일단 레벨 상한치는 20<br>

#### 맵<br>
 - 크기는 150x25

#### 핵<br>
- 1000의 체력<br>
- 한번에 양 팀의 미니언 하나씩 때림<br>
- 캐릭터를 때리지 않음<br>
- 캐릭터가 한 대 때릴 때 마다 레벨x10 씩 데미지 받음<br>
- 핵의 일정범위 안에 같은 팀의 미니언이 2웨이브(8마리)이상 있으면 미니언이 모두 죽고 핵은 200의 피해를 받음<br>
- 핵이 200의 피해를 받을 때 체력이 그 이하거나 같으면 미니언만 죽음

#### 미니언<br>
- 근거리와 원거리로 나뉘어짐<br>
- 게임 시작 30초 후 20초마다 근거리2, 원거리2마리의 미니언이 나옴<br>
- 근거리 미니언은 25+분당 10의 경험치를 줌<br>
- 원거리 미니언은 15+분당 7의 경험치를 줌<br>
- 근거리 미니언은 220+분당 45의 체력과 30+분당 6의 공격력을 지님<br>
- 원거리 미니언은 130+분당 20의 체력과 45+분당 10의 공격력을 지님<br>
- 핵의 공격에 2대 맞으면 사망함<br>
- 기지에서 생성됨
- 생성될 때 0.5초의 간격으로 하나씩 나옴
- 상대의 포탑이 있으면 포탑쪽으로 이동하고 포탑이 없으면 핵 쪽으로 이동함
- 이동하던 중 적 유닛을 만나면 이동을 멈추고 적 유닛을 공격
- 적 유닛이 죽거나 일정 범위에서 벗어날 때 까지 공격함
- 적 캐릭터가 기본공격을 하면 적 캐릭터를 공격함

#### 기지
- 맵의 각 끝에 있음
- 캐릭터와 미니언이 생성되는 곳
- 캐릭터가 기지 안에 있으면 체력과 마나가 회복됨
- 유닛은 다른 팀의 기지에 닿을 수 없음

#### 수풀
- 수풀 안으로 들어갈 시 유닛의 모습이 보이지 않음
- 같은 수풀 안 유닛들은 서로를 볼 수 있음
- 수풀 안에서의 공격은 이펙트만 보임

## 캐릭터 공통 특성<br>
- 체력<br>
- 스킬에 소모하는 자원<br>
- 기본 공격이 주는 피해<br>
- 초당 기본 공격을 하는 속도<br>
- 초당 몇의 좌표를 이동하는지 알려줌(이동속도)<br>
- 기본 공격이 얼마의 범위를 가지는지(사거리)<br>
- 쓰지 않아도 항상 발동되어있는 스킬(패시브)<br>
- 사용하는 스킬 3개<br>
- 레벨<br>
- 레벨을 올리는데 필요한 경험치<br>
- 체력, 자원, 공격력, 공격속도, 이동속도를 각각 몇 올렸는지<br>
- 가지고 있는 스탯 포인트<br>
- 몇 번 적 캐릭터를 죽였는지<br>
- 몇 번 적 캐릭터에게 죽었는지<br>
 
 적을 공격할 때 공격 사거리 밖에 있으면 최대 사거리에 가서 적을 공격<br>
 공격 중 적이 사거리 밖으로 나가도 그대로 공격 실행<br>
 타게팅 스킬도 동일<br>
 
 기본공격 사거리는 반지름크기
 이동속도는 초당 좌표 이동거리 
 
## 각 캐릭터

#### 검투사(체력: 580, 분노: 100, 기본공격력: 69, 공격속도: 초당 0.75, 사거리: 2, 이동속도: 3)<br>
- 패시브(분노): 이 캐릭터는 마나대신 분노를 사용합니다. 스킬을 캐릭터에게 맞출 때 마다 분노가 25씩 오르며, 분노가 100이 되면 10초 동안 주는 피해와 공격속도가 30% 증가하며, 입힌 피해의 20%를 회복하며, 모든 스킬의 쿨타임이 50%감소합니다. 5초 동안 분노를 쌓지 못할 시, 분노를 모두 잃습니다. 이 캐릭터는 자원 스탯을 올리지 못합니다.<br>
- 스킬 1(횡베기) : 전방 60'의 적들에게 70+(15x공격력 스탯%)의 피해를 줍니다. 사거리: 3, 쿨타임 8초<br>
- 스킬 2(내려찍기): 무기로 땅을 찍어 직선거리의 적들에게 75+(20x공격력 스탯%)의 피해를 줍니다. 범위: 1, 사거리: 2.5, 쿨타임 6초<br>
- 스킬 3(지면강타): 전방으로 돌진하며 힘을 모은 후 땅을 내려찍어 원형 범위의 적들에게 85+(20x공격력 스탯%)의 피해를 줍니다. 돌진거리: 5, 범위: 반지름: 2. 쿨타임 12초<br>

#### 고양이(체력: 520, 마나: 150, 기본공격력: 71, 공격속도: 초당 0.88, 사거리: 1.75, 이동속도: 3.3)<br>
- 패시브(유연함): 기본공격을 4대 맞추면 다음에 들어오는 공격을 회피합니다.
- 스킬 1(발목 긋기) 40마나: 전방 45'거리에 있는 적들의 발목을 그어 80+(15x공격력 스탯%)의 피해를 입히고 1초동안 40% 둔화시킵니다. 사거리: 2, 쿨타임 7초<br>
- 스킬 2(찌르기) 항시발동: 매 4번째 기본공격마다 직선거리의 적들에게 기본공격력의 150% 만큼의 데미지를 줍니다. 이 공격은 기본공격으로 적용됩니다. 너비: 0.75, 거리: 2.25<br>
- 스킬 3(달려!) 30마나: 자신을 포함한 일정 거리 이내의 아군의 이동속도를 30% 증가시킵니다. 반지름: 4, 쿨타임 9초 <br>

#### 골렘(체력: 630, 마나: 200, 기본공격력: 83, 공격속도: 초당 0.67, 사거리: 1.5, 이동속도: 2.8)<br>
- 패시브(단단함): 피해를 10% 덜 받습니다. 적 캐릭터에게 스킬을 맞출 때 마다 5%의 피해감소를 추가로 얻습니다. 이 효과는 6번까지 중첩됩니다. 4초 동안 적 캐릭터에게 스킬을 맞추지 못하면 중첩을 모두 잃습니다.<br>
- 스킬 1(후려치기) 25마나: 단일 대상에게 80+(최대 체력의 6%)+(5x공격력 스탯%)의 피해를 주고 대상을 35% 둔화시킵니다. 사거리: 1.75, 쿨타임 3초<br>
- 스킬 2(땅찍기) 30마나: 땅을 내리쳐 직선 대상에게 90+(최대 체력의 4.5%)+(10x공격력 스탯%)의 피해를 줍니다. 너비: 2.5, 거리: 2.5, 쿨타임 5초<br>
- 스킬 3(낙석) 50마나: 몸을 날려 대상 지점으로 날아갑니다. 도착 지점의 적은 75+(최대 체력의 10%)+(10x공격력 스탯%)의 피해를 받고 1초동안 기절합니다. 날아가는 거리: 7, 도착지점 반지름: 2, 쿨타임 16초<br>

#### 광대(체력: 600, 광기: 50, 기본 공격력: 80, 공격속도 초당 0.68, 사거리: 1.5, 이동속도: 2.8)<br>
- 패시브(광기): 이 캐릭터는 기본공격을 맞출 때 마다 10, 스킬을 맞출 때 마다 15의 광기를 얻으며 광기가 50이 되면 다음 스킬이 강화됩니다. 5초 동안 광기를 쌓지 못하면 모든 광기가 사라집니다.<br>
- 스킬 1(말뚝박기) 15광기: 망치를 크게 내리쳐 원형범위의 적들에게 80+(10x공격력 스탯%)의 피해를 주고 1초 동안30% 둔화시킵니다. 강화 시 20%의 추가 데미지를 주고 적을 둔화시키는 대신 기절시킵니다. 사거리: 1.5, 반지름: 1, 쿨타임 7초
- 스킬 2(차내기) 15광기: 적 하나를 발로 차 뒤로 밀쳐내고 65+(15x공격력 스탯%)의 피해를 줍니다. 밀쳐지는 유닛에게 맞은 적은 동일한 피해를 입고 1초 동안 25% 둔화됩니다. 강화 시 15%의 피해를 더 주고 밀쳐낸 적과 밀쳐지는 유닛에게 맞은 적은 모두 1초 동안 기절합니다. 사거리: 1.5, 밀쳐지는 거리: 3, 쿨타임 8초
- 스킬 3(망치 휘두르기) 15광기: 1초 동안 정신을 집중한 후 전방 90'의 적들에게 90+(20x공격력 스탯%)의 피해를 줍니다. 강화 시 피해가 두배가 됩니다.  사거리: 2.5, 쿨타임 10초

#### 궁수(체력: 320, 마나: 220, 기본 공격력: 78, 공격속도: 초당 1.2, 사거리: 5, 이동속도: 2.8)<br>
- 패시브(약점공격): 같은 대상을 공격하면 10%의 추가 피해를 입힙니다.<br>
- 스킬 1(화살비) 50마나: 화살을 뿌려 0.5초당 45+(15x공격력 스탯%)의 피해를 주는 원형 구역을 3초동안 만듭니다. 사거리: 5.5, 반지름: 1, 쿨타임 8초<br>
- 스킬 2(집중) 30마나: 7초 동안 패시브의 효과가 2배가 되고 공격력이 20+(10x공격력 스탯%), 공격속도가 30+(10x공격속도 스탯)% 오릅니다. 쿨타임 12초<br>
- 스킬 3(발차기) 25마나: 적 하나를 발로 차 80+(20x공격력 스탯%)의 피해를 주고 밀칩니다. 밀쳐진 적은 1초동안 이동속도가 30% 감소합니다. 사거리: 1.5, 쿨타임 6초<br>

#### 기마병(체력 650, 마나 170, 기본 공격력: 77, 공격속도: 초당 0.81, 사거리: 2, 이동속도: 3.3)<br>
- 패시브(승마): 이 캐릭터는 한 방향으로 움직일수록 이동속도가 점점 빨라져 최대 30%까지 이동속도가 증가합니다.<br>
- 스킬 1(랜스 찌르기) 30마나: 전방의 적들을 찔러 70+(15x공격력 스탯%)의 피해를 주고 찔린 적들을 사거리의 끝으로 밀칩니다. 너비: 0.75, 거리: 2.5, 쿨타임 6초<br>
- 스킬 2(머리 찍기) 35마나: 전방의 적들의 머리를 내리쳐 1초동안 기절시키고 60+(20x공격력 스탯%)의 피해를 줍니다. 너비: 0.75, 사거리: 2쿨타임 8초<br>
- 스킬 3(전력질주) 40마나: 전력으로 말을 몰아 이동속도가 40% 증가하고 다음 기본공격이 40x(이동속도)의 추가 피해를 줍니다. 쿨타임 9초<br>

#### 리자드맨(체력: 540, 기본 공격력: 71, 공격속도: 초당 0.7, 사거리: 1.5, 이동속도: 3)<br>
- 패시브(고대의 존재): 이 캐릭터는 자원을 소모하지 않으며, 자원 스탯을 올릴 경우 공격력 스탯으로 적용됩니다.<br>
- 스킬 1(관통 찌르기): 돌진하며 창으로 전방의 적을 관통해 65+(15x공격력 스탯%)의 피해를 입힙니다. 너비: 1, 사거리: 4, 쿨타임 6초<br>
- 스킬 2(돌려베기): 원형 범위의 적을 공격해 70+(15x공격력 스탯%)의 피해를 입힙니다. 반지름: 1.5, 쿨타임 5초<br>
- 스킬 3(공중제비): 공중제비를 돌아 0.5초 동안 모든 공격을 회피합니다. 쿨타임 7초<br>

#### 로가릭(체력 10: 마나: 600, 기본 공격력: 72, 공격속도: 초당 0.91, 사거리: 3, 이동속도: 3)
- 패시브(마나실드): 피해를 받으면 체력이 줄어드는 대신 피해만큼 마나를 소모합니다. 피해가 마나보다 크면 발동하지 않습니다.
- 스킬 1(마나 화살) 40마나: 마나로 이뤄진 화살을 발사해 적들을 관통하며70+(20x공격력 스탯%) 의 피해를 줍니다. 너비: 0.5, 사거리: 4.5, 쿨타임 3초
- 스킬 2(마나 폭탄) 70마나: 불안정한 마나 덩어리를 발사해 원형 범위의 적들에게 85+(25x공격력 스탯%)의 피해를 주고 자신은 뒤로 밀려납니다. 사거리: 5, 폭발 반지름: 1, 밀려나는 거리: 3, 쿨타임 8초
- 스킬 3(마나 폭주) 온/오프: 모든 공격이 현재 마나의 10%를 추가로 소모하지만 50%의 추가 피해를 줍니다. 쿨타임 1초
