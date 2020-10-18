& docker-compose exec config sh -c "mongo < /scripts/init-cfg.js"
& docker-compose exec shardA sh -c "mongo < /scripts/init-shardA.js"
& docker-compose exec shardB sh -c "mongo < /scripts/init-shardB.js"
& docker-compose exec shardC sh -c "mongo < /scripts/init-shardC.js"
sleep 10
& docker-compose exec router sh -c "mongo < /scripts/init-router.js"