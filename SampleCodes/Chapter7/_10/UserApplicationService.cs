﻿namespace _10
{
    public class UserApplicationService
    {
        private readonly IUserRepository userRepository;

        public UserApplicationService()
        {
            // ServiceLocator를 통해 필요한 인스턴스를 받음
            this.userRepository = ServiceLocator.Resolve<IUserRepository>();
        }

        public UserData Get(string userId)
        {
            var targetId = new UserId(userId);
            var user = userRepository.Find(targetId);

            if (user == null)
            {
                return null;
            }

            var userData = new UserData(user);
            return userData;
        }
    }
}
