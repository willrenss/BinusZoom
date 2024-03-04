namespace BinusZoom.Service.ZoomService.DTO;

public class PastMeetingParticipantsDTO
{
    public string nextPageToken { get; set; }
    public int pageCount { get; set; }
    public int pageSize { get; set; }
    public int totalRecords { get; set; }
    public List<Participants> participants { get; set; }
    
}

public class Participants
{
    public String Id { get; set; }
    public String Name { get; set; }
    public String UserId { get; set; }
    public String RegistrantId { get; set; }
    public String UserEmail { get; set; }
    public DateTimeOffset JoinTime { get; set; }
    public DateTimeOffset LeaveTime { get; set; }
    public int Duration { get; set; }
    public bool Failover { get; set; }
    public String Status { get; set; }
    
}

// {
// "next_page_token": "Tva2CuIdTgsv8wAnhyAdU3m06Y2HuLQtlh3",
// "page_count": 1,
// "page_size": 30,
// "total_records": 1,
// "participants": [
// {
//     "id": "30R7kT7bTIKSNUFEuH_Qlg",
//     "name": "Jill Chill",
//     "user_id": "27423744",
//     "registrant_id": "_f08HhPJS82MIVLuuFaJPg",
//     "user_email": "jchill@example.com",
//     "join_time": "2022-03-23T06:58:09Z",
//     "leave_time": "2022-03-23T07:02:28Z",
//     "duration": 259,
//     "failover": false,
//     "status": "in_meeting"
// }
// ]
// }