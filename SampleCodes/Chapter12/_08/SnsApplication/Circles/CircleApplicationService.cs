﻿using System.Transactions;
using _08.SnsApplication.Circles.Create;
using _08.SnsApplication.Circles.Join;
using _08.SnsApplication.Users;
using _08.SnsDomain.Models.Circles;
using _08.SnsDomain.Models.Users;

namespace _08.SnsApplication.Circles
{
    public class CircleApplicationService
    {
        private readonly ICircleFactory circleFactory;
        private readonly ICircleRepository circleRepository;
        private readonly CircleService circleService;
        private readonly IUserRepository userRepository;

        public CircleApplicationService(
            ICircleFactory circleFactory,
            ICircleRepository circleRepository,
            CircleService circleService,
            IUserRepository userRepository)
        {
            this.circleFactory = circleFactory;
            this.circleRepository = circleRepository;
            this.circleService = circleService;
            this.userRepository = userRepository;
        }

        public void Create(CircleCreateCommand command)
        {
            using (var transaction = new TransactionScope())
            {
                var ownerId = new UserId(command.UserId);
                var owner = userRepository.Find(ownerId);
                if (owner == null)
                {
                    throw new UserNotFoundException(ownerId, "서클장이 될 사용자가 없음");
                }

                var name = new CircleName(command.Name);
                var circle = circleFactory.Create(name, owner);
                if (circleService.Exists(circle))
                {
                    throw new CanNotRegisterCircleException(circle, "이미 등록된 서클임");
                }

                circleRepository.Save(circle);
                transaction.Complete();
            }
        }

        public void Join(CircleJoinCommand command)
        {
            using (var transaction = new TransactionScope())
            {
                var memberId = new UserId(command.UserId);
                var member = userRepository.Find(memberId);
                if (member == null)
                {
                    throw new UserNotFoundException(memberId, "사용자를 찾지 못했음");
                }

                var id = new CircleId(command.CircleId);
                var circle = circleRepository.Find(id);
                if (circle == null)
                {
                    throw new CircleNotFoundException(id, "서클을 찾지 못했음");
                }

                if (circle.IsFull())
                {
                    throw new CircleFullException(id);
                }

                circle.Join(member);
                circleRepository.Save(circle);
                transaction.Complete();
            }
        }
    }
}
