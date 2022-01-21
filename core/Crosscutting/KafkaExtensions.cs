﻿using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Streamiz.Kafka.Net.Crosscutting
{
    internal static class KafkaExtensions
    {
        internal static List<WatermarkOffsetsByTopicPartition> GetWatermarkOffsets<K, V>(this IConsumer<K, V> consumer)
        {
            List<WatermarkOffsetsByTopicPartition> l = new List<WatermarkOffsetsByTopicPartition>();
            foreach (var p in consumer.Assignment)
            {
                var w = consumer.GetWatermarkOffsets(p);
                l.Add(new WatermarkOffsetsByTopicPartition(p.Topic, p.Partition, w.Low, w.High));
            }
            return l;
        }

        internal static int? PartitionCountForTopic(this Metadata metadata, string topic)
        {
            var topicInfo = metadata.Topics.FirstOrDefault(t => t.Topic.Equals(topic));
            return topicInfo?.Partitions.Count;
        }
        
        // NOT USED FOR MOMENT
        //internal static IEnumerable<PartitionMetadata> PartitionsForTopic(this Metadata clusterMetadata, string topic)
        //{
        //    if (clusterMetadata.Topics.Any(t => t.Topic.Equals(topic)))
        //    {
        //        return clusterMetadata.Topics.Find(t => t.Topic.Equals(topic)).Partitions;
        //    }
        //    else
        //    {
        //        return new List<PartitionMetadata>();
        //    }
        //}

        internal static IEnumerable<ConsumeResult<K, V>> ConsumeRecords<K, V>(this IConsumer<K, V> consumer, TimeSpan timeout)
            => ConsumeRecords(consumer, timeout, 500);

        internal static IEnumerable<ConsumeResult<K, V>> ConsumeRecords<K, V>(this IConsumer<K, V> consumer, TimeSpan timeout, long maxRecords)
        {
            List<ConsumeResult<K, V>> records = new List<ConsumeResult<K, V>>();
            DateTime dt = DateTime.Now;
            TimeSpan ts = TimeSpan.Zero;
            do
            {
                var r = consumer.Consume(ts);
                if (r != null)
                {
                    records.Add(r);
                }
                else
                {
                    // TODO : make log(N) to increse progressivly timeout consumption
                    ts = (dt.Add(timeout) - DateTime.Now);
                }

                if (records.Count >= maxRecords)
                    break;
            } while (dt.Add(timeout) > DateTime.Now);

            return records;
        }
    }
}
