using UnityEditor;
using UnityEngine;

public static class CreateNDDDialogueAssets
{
    private const string DIALOGUE_PATH = "Assets/ScriptableObjects/Dialogue";

    private static SpeakerSO _speakerMC;
    private static SpeakerSO _speakerHuyenMac;
    private static SpeakerSO _speakerSibling;
    private static SpeakerSO _speakerSuGia;
    private static SpeakerSO _speakerThoiHanTung;
    private static SpeakerSO _speakerTheTu;
    private static SpeakerSO _speakerTanTu;
    private static SpeakerSO _speakerHaiHoang;
    private static SpeakerSO _speakerChim;
    private static SpeakerSO _speakerTeTheTonGia;
    private static SpeakerSO _speakerLucNhiMaVien;
    private static SpeakerSO _speakerTangBinh;
    private static SpeakerSO _speakerNhacVoHa;
    private static SpeakerSO _speakerNoLe;
    private static SpeakerSO _speakerDanLang;
    private static SpeakerSO _speakerCuongTin;

    [MenuItem("Tools/NDD/Create All NDD Assets")]
    public static void CreateAll()
    {
        CreateAllSpeakers();
        CreateTutorialDialogues();
        CreateAct1Dialogues();
        CreateAct2Dialogues();
        CreateAct3Dialogues();
        CreateAct4Dialogues();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("All NDD dialogue assets created successfully.");
    }

    [MenuItem("Tools/NDD/Create NDD Speakers")]
    public static void CreateAllSpeakers()
    {
        if (!AssetDatabase.IsValidFolder(DIALOGUE_PATH))
            AssetDatabase.CreateFolder("Assets/ScriptableObjects", "Dialogue");

        _speakerMC = CreateSpeaker("NDD_MC", "Tiểu Vũ", new Color(0.25f, 0.65f, 1f, 1f));
        _speakerHuyenMac = CreateSpeaker("NDD_HuyenMac", "Huyền Mặc", new Color(0.75f, 0.4f, 1f, 1f));
        _speakerSibling = CreateSpeaker("NDD_NguoiThan", "Người Thân", new Color(0.5f, 0.8f, 1f, 1f));
        _speakerSuGia = CreateSpeaker("NDD_SuGia", "Sứ Giả Thái Thượng Đạo Viện", new Color(1f, 0.2f, 0.2f, 1f));
        _speakerThoiHanTung = CreateSpeaker("NDD_ThoiHanTung", "Thôi Hàn Tùng", new Color(0.18f, 0.75f, 0.25f, 1f));
        _speakerTheTu = CreateSpeaker("NDD_TheTu", "Tàn Hồn Thê Tử", new Color(0.7f, 0.9f, 1f, 1f));
        _speakerTanTu = CreateSpeaker("NDD_TanTu", "NPC Tán Tu", new Color(0.6f, 0.6f, 0.6f, 1f));
        _speakerHaiHoang = CreateSpeaker("NDD_HaiHoang", "Thương Uyên Hải Hoàng", new Color(0.1f, 0.5f, 0.9f, 1f));
        _speakerChim = CreateSpeaker("NDD_Chim", "Linh Hồn Chú Chim", new Color(0.9f, 0.95f, 1f, 1f));
        _speakerTeTheTonGia = CreateSpeaker("NDD_TeTheTonGia", "Tế Thế Tôn Giả", new Color(1f, 0.85f, 0.2f, 1f));
        _speakerLucNhiMaVien = CreateSpeaker("NDD_LucNhiMaVien", "Lục Nhĩ Ma Viên", new Color(1f, 0.28f, 0.08f, 1f));
        _speakerTangBinh = CreateSpeaker("NDD_TangBinh", "NPC Tăng Binh", new Color(0.6f, 0.4f, 0.2f, 1f));
        _speakerNhacVoHa = CreateSpeaker("NDD_NhacVoHa", "Nhạc Vô Hà", new Color(0.5f, 0.5f, 0.55f, 1f));
        _speakerNoLe = CreateSpeaker("NDD_NoLe", "NPC Nô Lệ Cơ Khí", new Color(0.4f, 0.4f, 0.4f, 1f));
        _speakerDanLang = CreateSpeaker("NDD_DanLang", "NPC Dân Làng", new Color(0.7f, 0.65f, 0.5f, 1f));
        _speakerCuongTin = CreateSpeaker("NDD_CuongTin", "NPC Cuồng Tín", new Color(0.9f, 0.5f, 0.1f, 1f));

        Debug.Log("All NDD speakers created.");
    }

    private static SpeakerSO CreateSpeaker(string fileName, string displayName, Color color)
    {
        string path = $"{DIALOGUE_PATH}/{fileName}.asset";
        SpeakerSO existing = AssetDatabase.LoadAssetAtPath<SpeakerSO>(path);
        if (existing != null)
        {
            SerializedObject so = new SerializedObject(existing);
            so.FindProperty("speakerName").stringValue = displayName;
            so.FindProperty("highlightColor").colorValue = color;
            so.FindProperty("dimColor").colorValue = new Color(color.r, color.g, color.b, 0.3f);
            so.ApplyModifiedProperties();
            return existing;
        }

        SpeakerSO speaker = ScriptableObject.CreateInstance<SpeakerSO>();
        AssetDatabase.CreateAsset(speaker, path);
        SerializedObject so2 = new SerializedObject(speaker);
        so2.FindProperty("speakerName").stringValue = displayName;
        so2.FindProperty("highlightColor").colorValue = color;
        so2.FindProperty("dimColor").colorValue = new Color(color.r, color.g, color.b, 0.3f);
        so2.ApplyModifiedProperties();
        return speaker;
    }

    // ---- TUTORIAL DIALOGUES ----

    [MenuItem("Tools/NDD/Create Tutorial Dialogues")]
    public static void CreateTutorialDialogues()
    {
        EnsureSpeakers();

        CreateDialogue("NDD_Prologue_Night", new DialogueLineData[]
        {
            new(_speakerSibling, "Huynh/Tỷ ăn đi. Mai là Lễ Trắc Cốt rồi, phải có sức để chịu đựng áp lực trên đài ngọc. Cán sự bảo Trắc Cốt Bàn sẽ rút sinh lực rất mạnh."),
            new(_speakerMC, "Ta không đói. Muội/Đệ ăn cho hết đi. Sắc mặt kém quá, mai lỡ bọn giám khảo Thái Thượng Đạo Viện thấy chướng mắt lại đánh rớt từ vòng ngoài."),
            new(_speakerSibling, "Huynh/Tỷ nói xem... Thượng Giới thực sự có ánh mặt trời không? Lão khất cái đầu làng bảo ở trên đó, nước không có màu đen ngòm váng dầu, mà trong vắt. Tiên nhân không bao giờ phải bới rác tìm quặng linh thạch vụn để nộp tô..."),
            new(_speakerMC, "Chỉ cần ngày mai một trong hai chúng ta kiểm tra ra Tạp Linh Căn, đủ tiêu chuẩn làm tạp dịch ngoại môn, ta sẽ đưa nhà mình khỏi cái hố phân này."),
            new(_speakerSibling, "Nhưng nếu... nếu cả hai chúng ta đều là Phế Linh Căn thì sao? Tháng trước, nhà lão Tôn cả ba người con đều bị phán là Phế cốt. Bọn lính canh đày họ xuống hầm mỏ Huyết Cương Thạch, chưa đầy bảy ngày đã trả về ba bộ xương khô..."),
            new(_speakerMC, "Không có nếu như. Dù có phải liều mạng với lính gác, ta cũng không để muội/đệ bước nửa bước xuống hầm mỏ. Ngủ đi. Giữ sức."),
            new(_speakerSibling, "Chỗ bánh này đệ/muội cất đi... Ngày mai, đo cốt xong, chúng ta cùng ăn mừng nhé. Nhất định sẽ lên được Thượng Giới..."),
        });

        CreateDialogue("NDD_Prologue_Ceremony", new DialogueLineData[]
        {
            new(_speakerDanLang, "Khụ khụ... Năm nay thuế máu lại tăng. Nếu thằng cẩu tử nhà ta không đậu nổi Tạp Linh Căn, tháng sau cả nhà chỉ có nước xuống hầm quặng bốc vác cho cạn mạng."),
            new(_speakerDanLang, "Im miệng. Tiên nhân nghe thấy bây giờ. Mười năm trước có kẻ gian lận tuổi, cả xóm bị lột da treo lên cọc đấy."),
            new(_speakerSibling, "Đừng sợ. Huynh/Muội khỏe thế này, chắc chắn sẽ có Linh Căn. Hôm qua muội/huynh nằm mơ thấy hai chúng ta được cưỡi hạc bay lên chiếc thuyền kia. Lên đó rồi, nhà mình sẽ có cơm trắng ăn mỗi ngày."),
            new(_speakerSuGia, "KÍCH HOẠT TRẮC CỐT BÀN. TẤT CẢ PHÀM NHÂN, RẬP ĐẦU."),
            new(_speakerSuGia, "Ân điển? Các ngươi nghĩ Thiên Đạo ban phát sức mạnh từ thiện sao? Một đám giòi bọ. Khí hải dơ bẩn, cốt tủy hôi thối. Kẻ có Linh Căn, được lên thuyền làm chó săn cho Thượng Giới. Kẻ Phế Linh Căn, ngoan ngoãn ở lại làm phân bón cho Cây Thế Giới."),
            new(_speakerSuGia, "Ánh sáng này... Mạch máu tinh khiết, không một tì vết! THIÊN LINH CĂN! Hahaha! Tốt! Tốt lắm! Bay đâu, mang đứa trẻ này lên phòng thượng hạng, chuẩn bị Tẩy Tủy Trận!"),
            new(_speakerSibling, "Không! Ta không đi một mình! Còn người nhà của ta! Mau lên đây, đến lượt người rồi, mau đo cốt đi!"),
            new(_speakerSuGia, "Chó má còn đòi mặc cả? Cầm lấy."),
            new(_speakerSuGia, "Khí hải trống rỗng. Mạch môn bế tắc. Rác rưởi nguyên chất. Đánh gãy hai chân nó rồi ném ra sau bãi phế liệu, đừng để nó làm bẩn mắt vị 'Tiên nhân' mới của chúng ta."),
        });

        CreateDialogue("NDD_Prologue_Awakening", new DialogueLineData[]
        {
            new(_speakerHuyenMac, "Tỉnh dậy. Đừng giả chết nữa. Bọn chó săn của Thái Thượng Đạo Viện đánh gãy xương cốt, nhưng chưa bóp nát tâm mạch của ngươi đâu."),
            new(_speakerMC, "Ngươi... là thứ gì?"),
            new(_speakerHuyenMac, "Người nhà ngươi mang Thiên Linh Căn, đã bay lên chín tầng mây làm rồng làm phượng rồi. Còn ngươi? Phế Linh Căn, rác rưởi hạ đẳng. Một vạn kiếp nữa cũng không chạm được vào gót giày của bọn chúng."),
            new(_speakerMC, "Ta phải lên đó."),
            new(_speakerHuyenMac, "Bằng cái thân tàn tạ này? Khí hải của ngươi bế tắc, không thể hấp thụ linh khí đất trời. Con đường tu tiên chính đạo của ngươi đã đứt đoạn từ lúc sinh ra. Ngươi lấy cái gì để đòi người?"),
            new(_speakerHuyenMac, "Nhưng ta có một cách. Thiên Đạo không cho ngươi sinh cơ, thì ngươi có thể lấy của ta. Dám không?"),
            new(_speakerMC, "Được, ta sẽ đi lên và dẫm hết những kẻ coi thường ta dưới chân."),
        });

        CreateDialogue("NDD_Tutorial_Training", new DialogueLineData[]
        {
            new(_speakerHuyenMac, "Thiên Đạo đã vỡ, thân xác ngươi hiện tại quá yếu ớt để chịu đựng luồng chân khí này. Thử vận động đi. Nhìn kẻ thù giả lập phía trước kìa."),
            new(_speakerHuyenMac, "Tốt. Giờ thử dồn linh lực vào đòn đánh. Dùng kỹ năng để phá vỡ lớp giáp của nó."),
            new(_speakerHuyenMac, "Cơ thể đã quen dần rồi. Chuẩn bị đi, mùi sinh khí của ngươi vừa kéo khá nhiều sinh vật đến đấy."),
        });

        CreateDialogue("NDD_Tutorial_Survival", new DialogueLineData[]
        {
            new(_speakerHuyenMac, "Trận pháp giam cầm đã kích hoạt. Ngươi có 60 giây để sống sót."),
            new(_speakerMC, "Một đám ô hợp."),
        });

        CreateDialogue("NDD_Tutorial_Shop", new DialogueLineData[]
        {
            new(_speakerDanLang, "Hoan nghênh đến Quỷ Thị. Ở thời kỳ Mạt Pháp này, vàng bạc trần thế chỉ là giấy vụn. Linh thạch là thứ duy nhất có giá trị giao dịch."),
            new(_speakerHuyenMac, "Chuẩn bị xong chưa? Phía trước có kẻ đang đợi ngươi."),
        });

        CreateDialogue("NDD_Tutorial_BossPre", new DialogueLineData[]
        {
            new(_speakerSuGia, "Haha, một con chuột nhắt mới hôm nao còn phế linh căn giờ đã biết hấp thụ linh khí, xem ra trên người ngươi chắc chắn có bí mật, giao ra đây hoặc để ta tiêu diệt ngươi."),
            new(_speakerMC, "Bớt nhiều lời."),
        });

        CreateDialogue("NDD_Tutorial_BossPost", new DialogueLineData[]
        {
            new(_speakerHuyenMac, "Ngươi làm tốt lắm. Nhưng đó chỉ là con chó săn hạng thấp. Phía trước còn nhiều kẻ mạnh hơn. Đi nào."),
            new(_speakerMC, "Ta sẽ không dừng lại. Vì người thân của ta."),
        });

        Debug.Log("Tutorial dialogues created.");
    }

    // ---- ACT 1 DIALOGUES ----

    [MenuItem("Tools/NDD/Create Act 1 Dialogues")]
    public static void CreateAct1Dialogues()
    {
        EnsureSpeakers();

        CreateDialogue("NDD_Act1_Entrance", new DialogueLineData[]
        {
            new(_speakerHuyenMac, "Ngửi thấy mùi gì không? Mùi của sự sống giả tạo. Kẻ cai quản nơi này đang dùng Mộc Trấn Thiên Khí để cướp đoạt sinh cơ của vạn vật dưới đáy vực, bơm ngược lên trên. Cứ mỗi hơi thở của ngươi ở đây, kinh mạch sẽ bị xâm thực."),
            new(_speakerMC, "Kẻ nào có thể làm ra loại đại trận tà ác thế này?"),
            new(_speakerHuyenMac, "Thôi Hàn Tùng. Kẻ được người đời xưng tụng là Cổ Thụ Tôn Giả. Từng là một y sư cứu nhân độ thế. Thật nực cười."),
        });

        CreateDialogue("NDD_Act1_TanTu", new DialogueLineData[]
        {
            new(_speakerTanTu, "Tiểu hữu... đừng đi tiếp... phía trước không có đường sống đâu... Cổ Thụ Tôn Giả điên rồi..."),
            new(_speakerMC, "Ông ta đang bảo vệ thứ gì ở trung tâm đại trận?"),
            new(_speakerTanTu, "Một bóng ma... Năm đó, thê tử của ngài ấy bị Huyền Thiên Đạo Tôn hạ độc thủ. Để giữ lại một tia tàn hồn của nàng, ngài ấy tự biến mình thành lõi của Cây Thế Giới... hút máu của chúng ta... vắt kiệt tinh hoa của đất trời chỉ để nuôi một nụ hoa không bao giờ nở..."),
            new(_speakerTanTu, "Ngài ấy từng cứu mạng ta... giờ ta trả mạng cho ngài ấy... Giết ngài ấy đi... xin ngươi..."),
        });

        CreateDialogue("NDD_Act1_BossPre", new DialogueLineData[]
        {
            new(_speakerTheTu, "Chàng ơi... buông tay đi... Đừng giết thêm ai nữa... Thân xác thiếp đã mục nát, cớ sao chàng cứ bắt thiếp phải nhìn thế giới này khô héo vì mình?"),
            new(_speakerTheTu, "Sống lại trên núi xương sông máu... đó là ác quỷ, không phải là con người. Xin người đi đường, hãy dùng đao kiếm của ngươi, chặt đứt gông cùm cho phu quân ta. Chàng ấy... đã quá mệt mỏi rồi."),
        });

        CreateDialogue("NDD_Act1_BossFight", new DialogueLineData[]
        {
            new(_speakerThoiHanTung, "Lại một kẻ nữa đến nộp mạng. Sinh cơ của ngươi rất tinh khiết. Tốt lắm. Làm phân bón cho hoa của nàng ấy đi."),
            new(_speakerHuyenMac, "Hắn dựng Khiên Mộc rồi! Tấn công vào bản thể vô dụng thôi. Nhìn 8 nhánh rễ đang hút máu xung quanh kìa, chặt đứt chúng trước!"),
            new(_speakerThoiHanTung, "Kẻ nào dám đụng vào nàng! Ta sẽ rút cạn tủy sống của ngươi! Chỉ cần nàng mở mắt nhìn ta một lần nữa!", true, 2f),
            new(_speakerThoiHanTung, "Sắp được rồi... Chút nữa thôi! Uyển Nhi, đừng nhìn ta bằng ánh mắt đó! Bọn chúng chỉ là giun dế! Nàng mới là cả thế giới của ta!", true, 2f),
        });

        CreateDialogue("NDD_Act1_BossPost", new DialogueLineData[]
        {
            new(_speakerThoiHanTung, "Như Yên... Ta xin lỗi... Ta giữ được thiên hạ cho Đạo Tôn... nhưng không giữ nổi nàng... Bàn tay ta... bẩn quá rồi..."),
            new(_speakerTheTu, "Không sao... Chúng ta cuối cùng cũng được nghỉ ngơi rồi. Cảm ơn chàng."),
            new(_speakerThoiHanTung, "Phế Linh Căn.... Ngươi rất giống ta năm đó. Cầm lấy nó. Phá nát cái Thiên Không Thành chết tiệt kia. Đừng... để mất người ngươi yêu thương nhất."),
            new(_speakerHuyenMac, "Mộc Trấn Thiên Khí đã được thu hồi. Đi nào. Phía trước còn Đông Hải Cự Uyên đang chờ."),
        });

        Debug.Log("Act 1 dialogues created.");
    }

    // ---- ACT 2 DIALOGUES ----

    [MenuItem("Tools/NDD/Create Act 2 Dialogues")]
    public static void CreateAct2Dialogues()
    {
        EnsureSpeakers();

        CreateDialogue("NDD_Act2_Entrance", new DialogueLineData[]
        {
            new(_speakerChim, "Người lạ, đừng bước vào luồng hút kia. Áp suất nén sẽ nghiền nát cốt nhục của ngươi. Nơi này đã bị một kẻ từ Thượng Giới kiểm soát."),
            new(_speakerMC, "Kẻ nào đủ sức thao túng được cả Thủy Trấn Thiên Khí?"),
            new(_speakerChim, "Bằng huyễn thuật và con tin. Tên sứ giả Thượng Giới đó đã đánh lén, lột bỏ xác thịt ta và nhốt tàn hồn ta vào bọt nước này. Sau đó, hắn tạo ra một huyễn ảnh giả mạo ta, ngày đêm kêu gào đau đớn để uy hiếp phụ thân ta - Thương Uyên Hải Hoàng."),
            new(_speakerMC, "Ông ta không nhận ra đó là đồ giả sao? Hắn bắt ông ta làm gì?"),
            new(_speakerChim, "Thần trí phụ thân đã bị huyễn thuật làm cho điên loạn. Hắn lừa phụ thân rằng để cứu mạng ta, ngài phải dùng Thủy Trấn Thiên Khí gom toàn bộ nước của Đông Hải nén lại thành một Lõi Thủy Áp khổng lồ. Phụ thân đang tự tay vắt kiệt và giết chết đại dương này."),
            new(_speakerMC, "Dẫn đường đi. Ta sẽ chém nát cái huyễn ảnh đó, cắt đứt thuật thao túng."),
        });

        CreateDialogue("NDD_Act2_BossPre", new DialogueLineData[]
        {
            new(_speakerChim, "Phụ hoàng... cứu con! Con nóng quá... Nén thêm nước đi, con cần thêm Thủy khí!"),
            new(_speakerHaiHoang, "Cố chịu đựng! Phụ hoàng sẽ gom cạn Đông Hải này, nén tất cả lại để giữ mạng cho con!"),
            new(_speakerMC, "Mở mắt ra mà nhìn, Hải Hoàng. Ông đang dồn sức mạnh để cung phụng cho một con ký sinh trùng Thượng Giới đấy."),
            new(_speakerHaiHoang, "Kẻ nào dám làm gián đoạn nghi thức? Bất cứ ai ngăn cản ta cứu con... đều phải chết!"),
        });

        CreateDialogue("NDD_Act2_BossPost", new DialogueLineData[]
        {
            new(_speakerHaiHoang, "Đứa trẻ đó... không phải là con ta... Huyễn thuật... Ta đã làm gì biển khơi thế này?"),
            new(_speakerMC, "Kẻ thù tước đoạt xác thịt, nhưng hồn phách con ông vẫn ở đây."),
            new(_speakerHaiHoang, "Con ta... Phụ hoàng đã mù lòa. Ngàn năm tu vi, lại bị một cái bẫy của Thượng Giới dắt mũi, hèn mạt tự tay nén cạn sinh cơ của Đông Hải."),
            new(_speakerHaiHoang, "Tội nghiệt của ta, ta dùng mạng này để bù đắp cho Đông Hải. Cầm lấy Thủy Trấn Thiên Khí. Đừng dừng lại. Hãy đâm xuyên cái Thượng Giới đó... đừng để bất kỳ ai bị bọn chúng đùa giỡn nhân tính như cha con ta nữa..."),
        });

        Debug.Log("Act 2 dialogues created.");
    }

    // ---- ACT 3 DIALOGUES ----

    [MenuItem("Tools/NDD/Create Act 3 Dialogues")]
    public static void CreateAct3Dialogues()
    {
        EnsureSpeakers();

        CreateDialogue("NDD_Act3_Entrance", new DialogueLineData[]
        {
            new(_speakerCuongTin, "Lửa nghiệp thanh tẩy... Công đức vô lượng... Tế Thế Tôn Giả sẽ đưa chúng ta thoát khỏi bể khổ Mạt Pháp. Nhảy xuống đi, các đạo hữu!"),
            new(_speakerMC, "Mở mắt ra! Lửa này đang đốt cháy lục phủ ngũ tạng, rút cạn hồn phách của các ngươi chứ không thanh tẩy cái gì cả."),
            new(_speakerCuongTin, "Ngươi là ác ma ngoại đạo! Tôn Giả đã ban cho chúng ta vinh hạnh được làm củi đốt để ngài duy trì Hỏa Trấn Thiên Khí. Chết vì ngài là niết bàn!"),
            new(_speakerMC, "Tín ngưỡng mù quáng. Kẻ kiểm soát Hỏa Trấn Thiên Khí đang dùng chính mạng sống của phàm nhân làm nhiên liệu."),
        });

        CreateDialogue("NDD_Act3_TangBinh", new DialogueLineData[]
        {
            new(_speakerTangBinh, "Đừng... đừng tiến lên đỉnh tháp. Kẻ ngự trị trên đó... không phải Phật, cũng không phải Tiên..."),
            new(_speakerMC, "Hắn thu thập tín ngưỡng để làm gì?"),
            new(_speakerTangBinh, "Để duy trì lớp vỏ bọc. Bản tôn của hắn là Lục Nhĩ Ma Viên - một con vượn quỷ khát máu. Hắn lén thả Huyết Sát Hài Nhi đi tàn sát, thiêu rụi các thôn làng... Rồi chính hắn mặc cà sa, hạ phàm giả làm người cứu rỗi."),
            new(_speakerTangBinh, "Hắn hút lấy sự biết ơn, sự tuyệt vọng của phàm nhân để đúc thành lớp kim thân hoàn mỹ. Phá vỡ lớp vỏ đó... ép con súc sinh đó lộ nguyên hình..."),
        });

        CreateDialogue("NDD_Act3_BossPre", new DialogueLineData[]
        {
            new(_speakerTeTheTonGia, "Biển khổ vô biên, quay đầu là bờ. Thí chủ sát nghiệp quá nặng, trên tay dính máu của Mộc và Thủy. Hãy buông bỏ đồ đao, bước vào lò bát quái của ta để gột rửa nghiệp chướng."),
            new(_speakerMC, "Gột rửa? Ngươi tự tay châm lửa đốt nhà người ta, rồi bắt họ dập đầu tạ ơn vì đã mang đến một xô nước? Tháo cái mặt nạ đạo mạo đó xuống đi, súc sinh."),
            new(_speakerTeTheTonGia, "Kẻ không hiểu Phật pháp, tất phải dùng nghiệp hỏa để độ hóa. Chết đi, tội đồ."),
        });

        CreateDialogue("NDD_Act3_BossFight", new DialogueLineData[]
        {
            new(_speakerTeTheTonGia, "Đại La Kim Ấn! Cúi đầu trước uy nghiêm của Thiên Đạo!"),
            new(_speakerMC, "Sát thương của lớp kim quang quá lớn. Phải liên tục tấn công vào các điểm mù, phá vỡ thanh sức bền để lột lớp vỏ bọc này."),
            new(_speakerLucNhiMaVien, "RÁC RƯỞI! Dám làm xước kim thân vạn năm của ta! Ta sẽ nhai nát sọ ngươi! Xé xác ngươi ra từng mảnh!", true, 2f),
            new(_speakerLucNhiMaVien, "Chạy đi đâu! Nếm thử ngọn lửa từ địa ngục đi!"),
        });

        CreateDialogue("NDD_Act3_BossPost", new DialogueLineData[]
        {
            new(_speakerLucNhiMaVien, "Không... Ta là đấng cứu thế... Bọn giun dế các ngươi... phải thờ phụng ta..."),
            new(_speakerMC, "Ngươi chỉ là một con khỉ làm trò tiêu khiển cho Đạo Tôn. Không hơn không kém."),
            new(_speakerHuyenMac, "Hỏa Trấn Thiên Khí đã được thu hồi. Trần của Phạn Âm Hỏa Trạch nứt toác rồi. Con đường lên Đỉnh Lò Rèn đã mở."),
        });

        Debug.Log("Act 3 dialogues created.");
    }

    // ---- ACT 4 DIALOGUES ----

    [MenuItem("Tools/NDD/Create Act 4 Dialogues")]
    public static void CreateAct4Dialogues()
    {
        EnsureSpeakers();

        CreateDialogue("NDD_Act4_Entrance", new DialogueLineData[]
        {
            new(_speakerNoLe, "Giết... rút... rút... Nó bắt chúng ta... chạy lò luyện... bằng sinh mệnh..."),
            new(_speakerMC, "Kim Trấn Thiên Khí được dùng để vận hành cái địa ngục này sao. Kẻ nào điều khiển lõi máy?"),
            new(_speakerNoLe, "Nhạc Vô Hà... Hắn từng là đại tướng thủ thành... giờ chỉ là một cỗ máy chém giết... không có tim... Đừng để kim khí... đồng hóa mạch máu..."),
        });

        CreateDialogue("NDD_Act4_BossPre", new DialogueLineData[]
        {
            new(_speakerNhacVoHa, "Phàm... nhân... TẤT... CẢ... PHẢI... CHẾT!"),
            new(_speakerMC, "Một đống sắt vụn biết sủa. Trả Kim Trấn Thiên Khí đây."),
        });

        CreateDialogue("NDD_Act4_BossPost", new DialogueLineData[]
        {
            new(_speakerNhacVoHa, "Đạo Tôn... ngài lừa ta... Lò rèn này... không rèn ra thần khí... Nó rèn ra... ác quỷ..."),
            new(_speakerMC, "Sắt vụn thì về với bãi rác đi."),
            new(_speakerHuyenMac, "Kim Trấn Thiên Khí đã về tay ngươi. Bệ phóng trung tâm đã kích hoạt. Chuẩn bị cho trận chiến cuối cùng."),
        });

        Debug.Log("Act 4 dialogues created.");
    }

    // ---- HELPERS ----

    private static void EnsureSpeakers()
    {
        if (_speakerMC != null) return;
        _speakerMC = LoadOrNull("NDD_MC");
        _speakerHuyenMac = LoadOrNull("NDD_HuyenMac");
        _speakerSibling = LoadOrNull("NDD_NguoiThan");
        _speakerSuGia = LoadOrNull("NDD_SuGia");
        _speakerThoiHanTung = LoadOrNull("NDD_ThoiHanTung");
        _speakerTheTu = LoadOrNull("NDD_TheTu");
        _speakerTanTu = LoadOrNull("NDD_TanTu");
        _speakerHaiHoang = LoadOrNull("NDD_HaiHoang");
        _speakerChim = LoadOrNull("NDD_Chim");
        _speakerTeTheTonGia = LoadOrNull("NDD_TeTheTonGia");
        _speakerLucNhiMaVien = LoadOrNull("NDD_LucNhiMaVien");
        _speakerTangBinh = LoadOrNull("NDD_TangBinh");
        _speakerNhacVoHa = LoadOrNull("NDD_NhacVoHa");
        _speakerNoLe = LoadOrNull("NDD_NoLe");
        _speakerDanLang = LoadOrNull("NDD_DanLang");
        _speakerCuongTin = LoadOrNull("NDD_CuongTin");
    }

    private static SpeakerSO LoadOrNull(string fileName)
    {
        return AssetDatabase.LoadAssetAtPath<SpeakerSO>($"{DIALOGUE_PATH}/{fileName}.asset");
    }

    private static void CreateDialogue(string fileName, DialogueLineData[] lines)
    {
        string path = $"{DIALOGUE_PATH}/{fileName}.asset";
        DialogueSO existing = AssetDatabase.LoadAssetAtPath<DialogueSO>(path);
        if (existing != null)
            AssetDatabase.DeleteAsset(path);

        DialogueSO dialogue = ScriptableObject.CreateInstance<DialogueSO>();
        AssetDatabase.CreateAsset(dialogue, path);

        SerializedObject so = new SerializedObject(dialogue);
        SerializedProperty linesProp = so.FindProperty("lines");
        linesProp.ClearArray();
        linesProp.arraySize = lines.Length;

        for (int i = 0; i < lines.Length; i++)
        {
            SerializedProperty line = linesProp.GetArrayElementAtIndex(i);
            line.FindPropertyRelative("speaker").objectReferenceValue = lines[i].speaker;
            line.FindPropertyRelative("text").stringValue = lines[i].text;
            line.FindPropertyRelative("autoAdvance").boolValue = lines[i].autoAdvance;
            line.FindPropertyRelative("autoAdvanceDelay").floatValue = lines[i].delay;
            line.FindPropertyRelative("onStartEvent").stringValue = lines[i].onStartEvent ?? "";
            line.FindPropertyRelative("onEndEvent").stringValue = lines[i].onEndEvent ?? "";
        }

        so.ApplyModifiedProperties();
    }

    private readonly struct DialogueLineData
    {
        public readonly SpeakerSO speaker;
        public readonly string text;
        public readonly bool autoAdvance;
        public readonly float delay;
        public readonly string onStartEvent;
        public readonly string onEndEvent;

        public DialogueLineData(SpeakerSO speaker, string text,
            bool autoAdvance = false, float delay = 0f,
            string onStartEvent = null, string onEndEvent = null)
        {
            this.speaker = speaker;
            this.text = text;
            this.autoAdvance = autoAdvance;
            this.delay = delay;
            this.onStartEvent = onStartEvent;
            this.onEndEvent = onEndEvent;
        }
    }
}
