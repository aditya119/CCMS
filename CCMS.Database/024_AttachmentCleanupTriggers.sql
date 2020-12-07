-------------------------------------------------------------------------
-- Create Triggers for Attachment cleanup
-------------------------------------------------------------------------


create or replace trigger t_detail_file_cleanup_update_caseactors
after update on case_actors
for each row
begin
	if :old.detail_file <> :new.detail_file then
        if :old.detail_file <> 0 then
            pkg_attachments.p_delete_attachment(:old.detail_file, :new.last_update_by);
        end if;
    end if;
end t_detail_file_cleanup_update_caseactors;
/

create or replace trigger t_judgement_file_cleanup_update_caseproceedings
after update on case_proceedings
for each row
begin
	if :old.judgement_file <> :new.judgement_file then
        if :old.judgement_file <> 0 then
            pkg_attachments.p_delete_attachment(:old.judgement_file, :new.last_update_by);
        end if;
    end if;
end t_judgement_file_cleanup_update_caseproceedings;
/