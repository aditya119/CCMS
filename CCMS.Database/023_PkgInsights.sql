-------------------------------------------------------------------------
-- PKG_INSIGHTS
-------------------------------------------------------------------------

CREATE OR REPLACE PACKAGE pkg_insights IS
-------------------------------------------------------------------------
    PROCEDURE p_pending_disposed_cases (
        pi_user_id  IN case_proceedings.assigned_to%type,
        po_cursor   OUT sys_refcursor
    );

    PROCEDURE p_parameterised_report (
        pi_csv_filter_params    IN varchar2,
        po_cursor               OUT sys_refcursor
    );

END pkg_insights;
/
-------------------------------------------------------------------------

-------------------------------------------------------------------------
CREATE OR REPLACE PACKAGE BODY pkg_insights IS
-------------------------------------------------------------------------
    PROCEDURE p_pending_disposed_cases (
        pi_user_id  IN case_proceedings.assigned_to%type,
        po_cursor   OUT sys_refcursor
    ) IS
    BEGIN
        open po_cursor for
            select
                nvl(
                    sum(
                        case
                            when pd.proceeding_decision_name in ('FINAL JUDGEMENT')
                            then 1
                            else 0
                        end), 0) disposed_off,
                nvl(
                    sum(
                        case
                            when pd.proceeding_decision_name not in ('FINAL JUDGEMENT')
                            then 1
                            else 0
                        end), 0) pending
            from
                court_cases cc,
                proceeding_decisions pd
            where   cc.case_status = pd.proceeding_decision_id
                and cc.deleted is null
                and pd.deleted is null
                and (0 = pi_user_id
                    or cc.case_id in (
                        select
                            case_id
                        from
                            case_proceedings
                        where   deleted is null
                            and assigned_to = pi_user_id
                    ));
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_pending_disposed_cases - pi_user_id: ' || pi_user_id
                || chr(10) || sqlerrm);
    END p_pending_disposed_cases;
-------------------------------------------------------------------------
    PROCEDURE p_parameterised_report (
        pi_csv_filter_params    IN varchar2,
        po_cursor               OUT sys_refcursor
    ) IS
        v_filter_list   dbms_utility.lname_array;
    /*************************************************************************************************************************  
        pi_csv_filter_params is a comma-separated string with data in the following order.
        All id values will have value '-1' when all ids are to be fetched.
        There shall be no space between any item:
        1) case_number
        2) location_id
        3) lawyer_id
        4) court_id
        5) proceeding_date_start (dd-MON-yyyy)
        6) proceeding_date_end (dd-MON-yyyy)
    **************************************************************************************************************************/
    BEGIN
        select item
        bulk collect into v_filter_list
        from (select
                regexp_substr(pi_csv_filter_params,'[^,]+', 1, level) item
              from dual
              connect by
                regexp_substr(pi_csv_filter_params, '[^,]+', 1, level) is not null);
        open po_cursor for
            select
                cc.case_id,
                cc.case_number,
                cc.appeal_number,
                ct.court_name,
                lc.location_name,
                to_char(cd.case_filed_on, 'yyyy') case_filed_on_year,
                lw.lawyer_fullname,
                cp.proceeding_date,
                pd.proceeding_decision_name proceeding_decision,
                cp.next_hearing_on
            from
                court_cases cc,
                case_dates cd,
                case_proceedings cp,
                lawyers lw,
                locations lc,
                courts ct,
                proceeding_decisions pd
            where   cc.case_id = cd.case_id
                and cc.case_id = cp.case_id
                and cc.location_id = lc.location_id
                and cc.lawyer_id = lw.lawyer_id
                and cc.court_id = ct.court_id
                and cp.proceeding_decision = pd.proceeding_decision_id
                and cc.deleted is null
                and cd.deleted is null
                and cp.deleted is null
                and ('-1' = v_filter_list(1) or cc.case_number = v_filter_list(1))
                and ('-1' = v_filter_list(2) or cc.location_id = v_filter_list(2))
                and ('-1' = v_filter_list(3) or cc.lawyer_id = v_filter_list(3))
                and ('-1' = v_filter_list(4) or cc.court_id = v_filter_list(4))
                and cp.proceeding_date between to_date(v_filter_list(5)) and to_date(v_filter_list(6));
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_parameterised_report - pi_csv_filter_params: ' || pi_csv_filter_params
                || chr(10) || sqlerrm);
    END p_parameterised_report;
-------------------------------------------------------------------------
END pkg_insights;
/
-------------------------------------------------------------------------