﻿using System;
using System.Collections.Generic;
using System.Linq;
using _18.SnsApplication.Circles.GetSummaries;
using _18.SnsDomain.Models.Circles;
using _18.SnsDomain.Models.Users;

namespace _18.SnsApplication.Circles
{
    public class CircleApplicationService
    {
        private readonly ICircleFactory circleFactory;
        private readonly ICircleRepository circleRepository;
        private readonly CircleService circleService;
        private readonly IUserRepository userRepository;
        private readonly DateTime now;

        public CircleApplicationService(
            ICircleFactory circleFactory,
            ICircleRepository circleRepository,
            CircleService circleService,
            IUserRepository userRepository,
            DateTime now)
        {
            this.circleFactory = circleFactory;
            this.circleRepository = circleRepository;
            this.circleService = circleService;
            this.userRepository = userRepository;
            this.now = now;
        }

        public CircleGetSummariesResult GetSummaries(CircleGetSummariesCommand command)
        {
            // 모든 서클의 목록을 받아옴
            var all = circleRepository.FindAll();
            // 페이징 처리
            var circles = all
                .Skip((command.Page - 1) * command.Size)
                .Take(command.Size);
            var summaries = new List<CircleSummaryData>();
            foreach(var circle in circles)
            {
                // 각 서클의 서클장에 해당하는 사용자 정보 검색
                var owner = userRepository.Find(circle.Owner);
                summaries.Add(new CircleSummaryData(circle.Id.Value, owner.Name.Value));
            }
            return new CircleGetSummariesResult(summaries);
        }
    }
}
