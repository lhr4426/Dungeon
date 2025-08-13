# 스파르타 내일배움캠프 유니티 11기 6주차 Dungeon


## 프로젝트 소개

## 기술 스택

| C# | .Net | Unity |
| :--------: | :--------: | :--------: |
|   ![csharp]    |   ![dotnet]    |   ![unity]    |

<br>

## 구현 기능

### 필수 기능
- [x] 기본 이동 및 점프 : 스탠다드 강의에서 배웠던 Input System의 Auto Generate Class를 활용해서 플레이어의 이동을 구성
- [x] 체력바 UI : 추가 UI와 동시 진행함
- [x] 동적 환경 조사 : Update문에서 Ray 쏴서 진행
- [x] 점프대 : ForceMode.Impulse를 사용함
- [x] 아이템 데이터 : SO 사용
- [x] 아이템 사용 : 작아지는 버섯

### 도전 기능
- [x] 추가 UI : 챌린지 강의에서 배웠던 옵저버 + 중재자 패턴을 활용해 플레이어의 상태를 Observable\<T\>로 관리하여 UI가 상태가 변경될 때에만 호출
- [x] 3인칭 시점 : 3인칭용 카메라 컨테이너를 따로 만듬
- [x] 움직이는 플랫폼 구현 : 시작점과 끝점을 정해두고 
- [x] 벽타기 및 매달리기 : IsGrounded 만든 것처럼 OnWall 을 만들어서 Move에서 이동하는 축을 다르게 함
- [x] 다양한 아이템 구현 : 도넛, 버섯, 단검을 새로 만듬
- [x] 장비 장착 : 단검 추가. 단검을 들면 속도가 빨라짐
- [x] 레이저 트랩 : 두 오브젝트의 센터를 구해서 그 센터에서 반대편 센터로 레이저를 쏘도록 함. (번외 : 몬스터는 데미지를 거의 바로 입고, 플레이어는 0.5초마다 입음)
- [x] 상호작용 가능한 오브젝트 표시 : 레이저 트랩이랑 연동해서 레이저를 끄고 킬 수 있게 함
- [x] 플랫폼 발사기 : PlatformShooter 클래스를 따로 만들어서 구현
- [x] 발전된 AI : 물을 Not Walkable에서 Water로 바꾸고 가중치를 5로 설정 + Obstacle 추가

<br>

## 배운 점 & 아쉬운 점

이번에 특강을 중간 중간 들으면서 배운 것들을 써보다 보니, 데이터를 어떻게 구성해야 할 지 고민하는 시간을 가질 수 있었습니다.  


<br>

## 개발 일지

### 소요 기간 : 3일

[개발 1일차](https://lhr4426.pages.dev/2025-%EC%8A%A4%ED%8C%8C%EB%A5%B4%ED%83%80-%EB%82%B4%EB%B0%B0%EC%BA%A0-%EC%9C%A0%EB%8B%88%ED%8B%B0-11%EA%B8%B0/%EB%B3%B8%EC%BA%A0%ED%94%84/%EB%82%B4%EC%9D%BC%EB%B0%B0%EC%9B%80%EC%BA%A0%ED%94%84-%EB%B3%B8%EC%BA%A0%ED%94%84---250811)
[개발 2일차](https://lhr4426.pages.dev/2025-%EC%8A%A4%ED%8C%8C%EB%A5%B4%ED%83%80-%EB%82%B4%EB%B0%B0%EC%BA%A0-%EC%9C%A0%EB%8B%88%ED%8B%B0-11%EA%B8%B0/%EB%B3%B8%EC%BA%A0%ED%94%84/%EB%82%B4%EC%9D%BC%EB%B0%B0%EC%9B%80%EC%BA%A0%ED%94%84-%EB%B3%B8%EC%BA%A0%ED%94%84---250812)
[개발 3일차](https://lhr4426.pages.dev/2025-%EC%8A%A4%ED%8C%8C%EB%A5%B4%ED%83%80-%EB%82%B4%EB%B0%B0%EC%BA%A0-%EC%9C%A0%EB%8B%88%ED%8B%B0-11%EA%B8%B0/%EB%B3%B8%EC%BA%A0%ED%94%84/%EB%82%B4%EC%9D%BC%EB%B0%B0%EC%9B%80%EC%BA%A0%ED%94%84-%EB%B3%B8%EC%BA%A0%ED%94%84---250813)

## 라이센스

MIT &copy; [LeeHyerim](mailto:hyerimlee4426@gmail.com)

<!-- Stack Icon Refernces -->

[csharp]: /Images/Csharp.png
[dotnet]: /Images/Dotnet.png
[unity]: /Images/Unity.png



