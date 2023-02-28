-- this was for a refactor but it was made pointless later on w.e. absolutely shit emulator
UPDATE furniture
SET height_adjustable = REPLACE(height_adjustable, ',', ';');

-- permissions
INSERT INTO `permissions_commands` (`command`, `group_id`, `subscription_id`) VALUES ('command_change_motto', 5, 0);
INSERT INTO `permissions_commands` (`command`, `group_id`) VALUES ('command_hit', 1);

-- bots, need to do other logics before continuing
ALTER TABLE `bots` MODIFY COLUMN `ai_type` enum ( 'generic', 'bartender', 'pet', 'nurse', 'thug', 'plug', 'police', 'gun_vendor', 'jury', 'quest', 'bodyguard', 'bodyguard_plus', 'car_seller' );
ALTER TABLE `bots_responses` MODIFY COLUMN `bot_ai` enum ( 'generic', 'bartender', 'pet', 'nurse', 'thug', 'plug', 'police', 'gun_vendor', 'jury', 'quest', 'bodyguard', 'bodyguard_plus', 'car_seller' );

INSERT INTO `bots_responses` (`id`, `bot_ai`, `chat_keywords`, `response_text`, `response_mode`, `response_beverage`) VALUES (7, 'plug', 'drugs,drug,weed,plug', 'You want that good shit? It\'s gonna cost ya', 'whisper', '0');
INSERT INTO `bots_responses` (`id`, `bot_ai`, `chat_keywords`, `response_text`, `response_mode`, `response_beverage`) VALUES (8, 'nurse', 'heal,nurse', 'How can I help you, {username}?', 'say', '0');
INSERT INTO `catalog_bot_presets` (`id`, `name`, `figure`, `gender`, `motto`, `ai_type`) VALUES (1000000109, 'Mary', 'lg-710-1408.hr-9534-33.sh-735-1408.hd-600-1.ch-675-1408.', 'F', 'Can sell you heals!', 'nurse');
INSERT INTO `catalog_bot_presets` (`id`, `name`, `figure`, `gender`, `motto`, `ai_type`) VALUES (1000000110, 'Jeremy', 'lg-275-1408.hr-105-42.ca-3292-110.ch-3185-64.hd-180-1370.sh-3068-64-110.', 'M', 'Your local neighborhood plug', 'plug');
INSERT INTO `catalog_items` (`id`, `page_id`, `item_id`, `catalog_name`, `cost_credits`, `cost_pixels`, `cost_diamonds`, `amount`, `limited_sells`, `limited_stack`, `offer_active`, `extradata`, `badge`, `offer_id`) VALUES (1000000109, 9, '1000000109', 'bot_nurse', 3, 0, 0, 1, 0, 0, '1', 'lg-710-1408.hr-9534-33.sh-735-1408.hd-600-1.ch-675-1408.', '', -1);
INSERT INTO `catalog_items` (`id`, `page_id`, `item_id`, `catalog_name`, `cost_credits`, `cost_pixels`, `cost_diamonds`, `amount`, `limited_sells`, `limited_stack`, `offer_active`, `extradata`, `badge`, `offer_id`) VALUES (1000000110, 9, '1000000110', 'bot_plug', 3, 0, 0, 1, 0, 0, '1', 'lg-275-1408.hr-105-42.ca-3292-110.ch-3185-64.hd-180-1370.sh-3068-64-110.', '', -1);
INSERT INTO `furniture` (`id`, `item_name`, `public_name`, `type`, `width`, `length`, `stack_height`, `can_stack`, `can_sit`, `is_walkable`, `sprite_id`, `allow_recycle`, `allow_trade`, `allow_marketplace_sell`, `allow_gift`, `allow_inventory_stack`, `interaction_type`, `behaviour_data`, `interaction_modes_count`, `vending_ids`, `height_adjustable`, `effect_id`, `wired_id`, `is_rare`, `clothing_id`, `extra_rot`) VALUES (1000000109, 'bot_nurse', 'nurse bot', 'r', 1, 1, 0, '1', '0', '0', 0, '1', '1', '1', '1', '1', 'default', 1, 1, '0', '0', 0, 0, '0', 0, '0');
INSERT INTO `furniture` (`id`, `item_name`, `public_name`, `type`, `width`, `length`, `stack_height`, `can_stack`, `can_sit`, `is_walkable`, `sprite_id`, `allow_recycle`, `allow_trade`, `allow_marketplace_sell`, `allow_gift`, `allow_inventory_stack`, `interaction_type`, `behaviour_data`, `interaction_modes_count`, `vending_ids`, `height_adjustable`, `effect_id`, `wired_id`, `is_rare`, `clothing_id`, `extra_rot`) VALUES (1000000110, 'bot_plug', 'plug bot', 'r', 1, 1, 0, '1', '0', '0', 0, '1', '1', '1', '1', '1', 'default', 0, 1, '0', '0', 0, 0, '0', 0, '0');

-- user datas
ALTER TABLE `users`
ADD `health` int(11) NOT NULL DEFAULT '100';

ALTER TABLE `user_stats`
ADD `combat_level` int(11) NOT NULL DEFAULT '1';

-- rooms
ALTER TABLE `rooms`
ADD `combat_enabled` smallint(4) NOT NULL DEFAULT '1';


