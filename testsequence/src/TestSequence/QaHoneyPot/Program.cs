namespace QaHoneypot
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     The data in this class is for negative testing of accessibility between accounts.  
    /// </summary>
    public class Honeypot
    {
        public static List<string> UserRecordNumber()
        {
            return new List<string>
                       {
                           "UserRecordNumber|MO686770",
                           "UserRecordNumber|MG875244",
                           "UserRecordNumber|EG629610",
                           "UserRecordNumber|BO958186",
                       };
        }

        public static List<string> SecretId()
        {
            return new List<string> { "SecretId|545S494" };
        }

        public static List<string> ParticipantId()
        {
            return new List<string> { "ParticipantId|54S2485", "ParticipantId|785402", "ParticipantId|5452486", "ParticipantId|828004" };
        }

        public static List<string> EventId()
        {
            return new List<string> { "EventId|4877535" };
        }

        public static List<string> AllId()
        {
            var allId = new List<string>();
            allId.AddRange(PatientRecordNumber());
            allId.AddRange(PatientId());
            allId.AddRange(SchedulerEventParticipantId());
            allId.AddRange(SchedulerEventId());

            return allId;
        }

        public static List<string> LoginEmail()
        {
            return new List<string> { "LoginEmail|honey_pot@test.com", "Email|honey_pot@test.com", "Email|test@test.com" };
        }

        public static List<string> AllGuids()
        {

            return new List<string>
                       {
                           "UserGuid|f0f48901-bb7a-41b5-b767-01bed5f9197a",
                           "UserGuid|77bebd70-f0d5-49e5-8d07-0d3c54efb5adc",
                           "UserGuid|32f17138-3663-42eb-9296-1f59dca625e6",
                           "UserGuid|4e43a9fd-e1ac-41e2-9f96-59da850477bc",
                           "UserGuid|39c5b8a7-cd05-4090-8f6e-3dae5f6f9ea6",
                           "UserGuid|50321a17-e32a-4457-a181-b46001b7b24c",
                           "UserGuid|1601caf0-8a7e-42ac-ac07-6f4e3dff9c91",
                           "UserGuid|b05e24a0-db56-4228-8531-9a66e95dc894",
                           "UserGuid|0340a650-6ef6-47c5-a101-96ed5fd2edc9",
                           "UserGuid|f1b3c0bd-a2dc-4f9e-a776-b6d12fe26b66",
                           "UserGuid|9192ee56-bb15-4c63-941b-fd642591eb0f",
                           "UserGuid|1aab23d1-9a0c-40b2-afed-ddf2b9830a90",
                           "UserGuid|27d7d881-13f4-4159-b060-ea06895c10b1",
                           "UserGuid|8161b9ba-2a54-4513-91ad-b166aa82e76b",
                           "UserGuid|48d1d24f-1d62-4f60-babe-d3ed0c19dd79",
                           "UserGuid|e395e9a8-4e08-4948-8464-6056d64c653f",
                           "UserGuid|468faa51-5bac-46eb-a400-3167da9aa2a3",
                           "UserInternalGuid|f0f48901-bb7a-41b5-b767-01bed5f9197a",
                           "UserInternalGuid|77bebd70-f0d5-49e5-8d07-0d3c54efb5adc",
                           "UserInternalGuid|32f17138-3663-42eb-9296-1f59dca625e6",
                           "UserInternalGuid|4e43a9fd-e1ac-41e2-9f96-59da850477bc",
                           "UserInternalGuid|39c5b8a7-cd05-4090-8f6e-3dae5f6f9ea6",
                           "UserInternalGuid|50321a17-e32a-4457-a181-b46001b7b24c",
                           "UserInternalGuid|1601caf0-8a7e-42ac-ac07-6f4e3dff9c91",
                           "UserInternalGuid|b05e24a0-db56-4228-8531-9a66e95dc894",
                           "UserInternalGuid|0340a650-6ef6-47c5-a101-96ed5fd2edc9",
                           "UserInternalGuid|f1b3c0bd-a2dc-4f9e-a776-b6d12fe26b66",
                           "UserInternalGuid|9192ee56-bb15-4c63-941b-fd642591eb0f",
                           "UserInternalGuid|1aab23d1-9a0c-40b2-afed-ddf2b9830a90",
                           "UserInternalGuid|27d7d881-13f4-4159-b060-ea06895c10b1",
                           "UserInternalGuid|8161b9ba-2a54-4513-91ad-b166aa82e76b",
                           "UserInternalGuid|48d1d24f-1d62-4f60-babe-d3ed0c19dd79",
                           "UserInternalGuid|e395e9a8-4e08-4948-8464-6056d64c653f",
                           "LoginGuid|36e8a5fe-f369-4901-b7e2-f527a4ced8d6",
                           "LoginGuid|a423271e-3ec9-4fd2-b80f-54cc885a2f48",
                           "LoginGuid|059cde27-1494-4645-8e4f-961f1d33c2a7",
                           "LoginGuid|b405e276-b6b3-4652-b421-63a9a4673684",
                           "LoginGuid|64d12712-71c4-4142-9730-4cc8aaade9b5",
                           "LoginGuid|92e7d8e2-cb4e-41b4-96cf-ef37630b7189",
                           "LoginGuid|13b6aa19-2221-4496-be88-4e916a9a0b18",
                           "LoginGuid|8a26119c-87d8-4f25-87c0-8595c9ab8ad7",
                           "LoginGuid|32e4f6cd-d40d-4bb7-b858-592aaa48dc40",
                           "LoginGuid|cf58ace5-bb0a-418c-8c56-a2c84afc1f30",
                           "LoginGuid|0b96571d-1e7a-41ba-a834-8f4ce5f5eaca",
                           "LoginGuid|bd2b5434-7166-4b5b-9d7e-98d80d6e14ca",
                           "LoginGuid|66da150c-36bb-43d8-a7c1-75994292c430",
                           "LoginGuid|771f28af-8211-44a2-9211-a3c0acccd425",
                           "LoginGuid|756ca34d-a11b-404d-8769-9ad64af8c9d8",
                           "LoginGuid|d9dc5323-3b56-423e-afaf-ed671162f440",
                           "LoginGuid|2559d9f9-589f-44f2-b824-7bda21e124d6",
                           "LoginGuid|bcf931c3-2a63-49f5-8876-244764497d2e",
                           "SecurityGuid|894519fb-86a6-44d5-8aa7-dd1305367c71",
                           "SecurityGuid|d2792e7f-3e45-49c3-af39-73501fa05b23",
                           "SecurityGuid|a69dcc81-9efb-4b3b-9650-ef895ab93aa2",
                           "SecurityGuid|e08d2be1-a5af-4e2c-9fd0-3e19ffed5cfb",
                           "SecurityGuid|9015d594-7b38-42bb-8067-9d8abc708481",
                           "SecurityGuid|6eb8dec5-16dc-4542-a77c-71ffe7643600",
                           "SecurityGuid|6ec4fbee-2396-40c7-aec2-6236a3a5030e",
                           "SecurityGuid|ee3f8b9a-25b8-421b-a8c2-3655fee532ed",
                           "TimeTypeGuid|40bff5d0-b6e1-470f-b42e-fbc97e6d2210",
                           "TimeTypeGuid|4a78d2bd-cf65-43b1-be51-0e48a41c76ca",
                           "TimeTypeGuid|87788abe-0585-41e9-9f09-62cb94fdae3e",
                           "TimeTypeGuid|e067a64a-b3da-42c1-b790-68d7bce25f33",
                           "TimeTypeGuid|ae12ae61-485d-4f1c-95c3-51c5d7dc9e0b",
                       };
        }

        public static List<Guid> AllGuids(GuidTypes guidTypes)
        {
            var result = new List<Guid>();

            foreach (var guidId in AllGuids())
            {
                var guidIdArray = guidId.Split('|');

                if (guidIdArray[0].ToLower().Contains(guidTypes.ToString().ToLower()))
                {
                    Guid guid;
                    Guid.TryParse(guidIdArray[1], out guid);
                    result.Add(guid);
                }
            }

            return result;
        }


        public enum GuidTypes
        {
            UserGuid,
            TimeTypeGuid,
            SecurityGuid,
            LoginGuid,
            UserInternalGuid,
            UserGuid
        }
    }
}