﻿using AdminServer.Helpers;
using AdminServer.ServiceInterface;
using Grpc.Net.Client;
using LogServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminServer.Service
{
    public class LogService : ILogService
    {
        private LoggerManager.LoggerManagerClient _clientRepository = new LoggerManager.LoggerManagerClient(GrpcChannel.ForAddress("https://localhost:5002"));

        public PaginatedResponse<string> GetLogs(int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return new PaginatedResponse<string>();
            }
            var response = _clientRepository.GetLogs(new EmptyMessage { });
            int offset = (page - 1) * pageSize;
            if (offset > response.Logs.Count)
            {
                return new PaginatedResponse<string> ();
            }

            int lessCount = response.Logs.Count - (page * pageSize);
            var count = lessCount >= 0 ? pageSize : Math.Abs(pageSize + lessCount);
            return new PaginatedResponse<string>
            {
                TotalElements = response.Logs.Count,
                TotalPages = (int)Math.Ceiling((double)response.Logs.Count / pageSize),
                Elements = response.Logs.ToList().GetRange(offset, count),
            };
        }
    }
}
